



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<SimpleVoidMethod>d__0'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       190 (0xbe)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0' V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_0060

        IL_000e:  nop
        IL_000f:  ldstr      "Before"
        IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0019:  nop
        IL_001a:  ldc.r8     1.
        IL_0023:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_0028:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_002d:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0032:  stloc.1
        IL_0033:  ldloca.s   V_1
        IL_0035:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_003a:  brtrue.s   IL_007c

        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.0
        IL_003e:  dup
        IL_003f:  stloc.0
        IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_0045:  ldarg.0
        IL_0046:  ldloc.1
        IL_0047:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__1'
        IL_004c:  ldarg.0
        IL_004d:  stloc.2
        IL_004e:  ldarg.0
        IL_004f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
        IL_0054:  ldloca.s   V_1
        IL_0056:  ldloca.s   V_2
        IL_0058:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                        !!1&)
        IL_005d:  nop
        IL_005e:  leave.s    IL_00bd

        IL_0060:  ldarg.0
        IL_0061:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__1'
        IL_0066:  stloc.1
        IL_0067:  ldarg.0
        IL_0068:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__1'
        IL_006d:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.m1
        IL_0075:  dup
        IL_0076:  stloc.0
        IL_0077:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_007c:  ldloca.s   V_1
        IL_007e:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0083:  nop
        IL_0084:  ldstr      "After"
        IL_0089:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_008e:  nop
        IL_008f:  leave.s    IL_00a9

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0091:  stloc.3
        IL_0092:  ldarg.0
        IL_0093:  ldc.i4.s   -2
        IL_0095:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_009a:  ldarg.0
        IL_009b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
        IL_00a0:  ldloc.3
        IL_00a1:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_00a6:  nop
        IL_00a7:  leave.s    IL_00bd

      }  // end handler
      IL_00a9:  ldarg.0
      IL_00aa:  ldc.i4.s   -2
      IL_00ac:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
      IL_00b1:  ldarg.0
      IL_00b2:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
      IL_00b7:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00bc:  nop
      IL_00bd:  ret
    } // end of method '<SimpleVoidMethod>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleVoidMethod>d__0'::SetStateMachine

  } // end of class '<SimpleVoidMethod>d__0'

  .class auto ansi sealed nested private beforefieldinit '<VoidMethodWithoutAwait>d__1'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<VoidMethodWithoutAwait>d__1'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       66 (0x42)
      .maxstack  2
      .locals init (int32 V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  nop
        IL_0008:  ldstr      "No Await"
        IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0012:  nop
        IL_0013:  leave.s    IL_002d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0015:  stloc.1
        IL_0016:  ldarg.0
        IL_0017:  ldc.i4.s   -2
        IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
        IL_001e:  ldarg.0
        IL_001f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
        IL_0024:  ldloc.1
        IL_0025:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_002a:  nop
        IL_002b:  leave.s    IL_0041

      }  // end handler
      IL_002d:  ldarg.0
      IL_002e:  ldc.i4.s   -2
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
      IL_0035:  ldarg.0
      IL_0036:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
      IL_003b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0040:  nop
      IL_0041:  ret
    } // end of method '<VoidMethodWithoutAwait>d__1'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<VoidMethodWithoutAwait>d__1'::SetStateMachine

  } // end of class '<VoidMethodWithoutAwait>d__1'

  .class auto ansi sealed nested private beforefieldinit '<EmptyVoidMethod>d__2'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<EmptyVoidMethod>d__2'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (int32 V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  nop
        IL_0008:  leave.s    IL_0022

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_000a:  stloc.1
        IL_000b:  ldarg.0
        IL_000c:  ldc.i4.s   -2
        IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
        IL_0013:  ldarg.0
        IL_0014:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
        IL_0019:  ldloc.1
        IL_001a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_001f:  nop
        IL_0020:  leave.s    IL_0036

      }  // end handler
      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.s   -2
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
      IL_002a:  ldarg.0
      IL_002b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
      IL_0030:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0035:  nop
      IL_0036:  ret
    } // end of method '<EmptyVoidMethod>d__2'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<EmptyVoidMethod>d__2'::SetStateMachine

  } // end of class '<EmptyVoidMethod>d__2'

  .class auto ansi sealed nested private beforefieldinit '<AwaitYield>d__3'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<AwaitYield>d__3'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       159 (0x9f)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_004a

        IL_000e:  nop
        IL_000f:  call       valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable [mscorlib]System.Threading.Tasks.Task::Yield()
        IL_0014:  stloc.2
        IL_0015:  ldloca.s   V_2
        IL_0017:  call       instance valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter [mscorlib]System.Runtime.CompilerServices.YieldAwaitable::GetAwaiter()
        IL_001c:  stloc.1
        IL_001d:  ldloca.s   V_1
        IL_001f:  call       instance bool [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::get_IsCompleted()
        IL_0024:  brtrue.s   IL_0066

        IL_0026:  ldarg.0
        IL_0027:  ldc.i4.0
        IL_0028:  dup
        IL_0029:  stloc.0
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
        IL_002f:  ldarg.0
        IL_0030:  ldloc.1
        IL_0031:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>u__1'
        IL_0036:  ldarg.0
        IL_0037:  stloc.3
        IL_0038:  ldarg.0
        IL_0039:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
        IL_003e:  ldloca.s   V_1
        IL_0040:  ldloca.s   V_3
        IL_0042:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'>(!!0&,
                                                                                                                                                                                                                                                                                                  !!1&)
        IL_0047:  nop
        IL_0048:  leave.s    IL_009e

        IL_004a:  ldarg.0
        IL_004b:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>u__1'
        IL_0050:  stloc.1
        IL_0051:  ldarg.0
        IL_0052:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>u__1'
        IL_0057:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.m1
        IL_005f:  dup
        IL_0060:  stloc.0
        IL_0061:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
        IL_0066:  ldloca.s   V_1
        IL_0068:  call       instance void [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::GetResult()
        IL_006d:  nop
        IL_006e:  leave.s    IL_008a

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0070:  stloc.s    V_4
        IL_0072:  ldarg.0
        IL_0073:  ldc.i4.s   -2
        IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
        IL_007a:  ldarg.0
        IL_007b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
        IL_0080:  ldloc.s    V_4
        IL_0082:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0087:  nop
        IL_0088:  leave.s    IL_009e

      }  // end handler
      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.s   -2
      IL_008d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
      IL_0092:  ldarg.0
      IL_0093:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
      IL_0098:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_009d:  nop
      IL_009e:  ret
    } // end of method '<AwaitYield>d__3'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<AwaitYield>d__3'::SetStateMachine

  } // end of class '<AwaitYield>d__3'

  .class auto ansi sealed nested private beforefieldinit '<AwaitDefaultYieldAwaitable>d__4'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__4'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       160 (0xa0)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_004b

        IL_000e:  nop
        IL_000f:  ldloca.s   V_2
        IL_0011:  dup
        IL_0012:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable
        IL_0018:  call       instance valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter [mscorlib]System.Runtime.CompilerServices.YieldAwaitable::GetAwaiter()
        IL_001d:  stloc.1
        IL_001e:  ldloca.s   V_1
        IL_0020:  call       instance bool [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::get_IsCompleted()
        IL_0025:  brtrue.s   IL_0067

        IL_0027:  ldarg.0
        IL_0028:  ldc.i4.0
        IL_0029:  dup
        IL_002a:  stloc.0
        IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
        IL_0030:  ldarg.0
        IL_0031:  ldloc.1
        IL_0032:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>u__1'
        IL_0037:  ldarg.0
        IL_0038:  stloc.3
        IL_0039:  ldarg.0
        IL_003a:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
        IL_003f:  ldloca.s   V_1
        IL_0041:  ldloca.s   V_3
        IL_0043:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'>(!!0&,
                                                                                                                                                                                                                                                                                                                  !!1&)
        IL_0048:  nop
        IL_0049:  leave.s    IL_009f

        IL_004b:  ldarg.0
        IL_004c:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>u__1'
        IL_0051:  stloc.1
        IL_0052:  ldarg.0
        IL_0053:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>u__1'
        IL_0058:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.m1
        IL_0060:  dup
        IL_0061:  stloc.0
        IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
        IL_0067:  ldloca.s   V_1
        IL_0069:  call       instance void [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::GetResult()
        IL_006e:  nop
        IL_006f:  leave.s    IL_008b

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0071:  stloc.s    V_4
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.s   -2
        IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
        IL_007b:  ldarg.0
        IL_007c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
        IL_0081:  ldloc.s    V_4
        IL_0083:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0088:  nop
        IL_0089:  leave.s    IL_009f

      }  // end handler
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.s   -2
      IL_008e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
      IL_0093:  ldarg.0
      IL_0094:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
      IL_0099:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_009e:  nop
      IL_009f:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__4'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__4'::SetStateMachine

  } // end of class '<AwaitDefaultYieldAwaitable>d__4'

  .class auto ansi sealed nested private beforefieldinit '<AwaitDefaultHopToThreadPool>d__5'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<AwaitDefaultHopToThreadPool>d__5'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       160 (0xa0)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable V_1,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_004b

        IL_000e:  nop
        IL_000f:  ldloca.s   V_2
        IL_0011:  dup
        IL_0012:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
        IL_0018:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::GetAwaiter()
        IL_001d:  stloc.1
        IL_001e:  ldloca.s   V_1
        IL_0020:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::get_IsCompleted()
        IL_0025:  brtrue.s   IL_0067

        IL_0027:  ldarg.0
        IL_0028:  ldc.i4.0
        IL_0029:  dup
        IL_002a:  stloc.0
        IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
        IL_0030:  ldarg.0
        IL_0031:  ldloc.1
        IL_0032:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>u__1'
        IL_0037:  ldarg.0
        IL_0038:  stloc.3
        IL_0039:  ldarg.0
        IL_003a:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
        IL_003f:  ldloca.s   V_1
        IL_0041:  ldloca.s   V_3
        IL_0043:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitOnCompleted<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'>(!!0&,
                                                                                                                                                                                                                                                                                                              !!1&)
        IL_0048:  nop
        IL_0049:  leave.s    IL_009f

        IL_004b:  ldarg.0
        IL_004c:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>u__1'
        IL_0051:  stloc.1
        IL_0052:  ldarg.0
        IL_0053:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>u__1'
        IL_0058:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.m1
        IL_0060:  dup
        IL_0061:  stloc.0
        IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
        IL_0067:  ldloca.s   V_1
        IL_0069:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::GetResult()
        IL_006e:  nop
        IL_006f:  leave.s    IL_008b

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0071:  stloc.s    V_4
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.s   -2
        IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
        IL_007b:  ldarg.0
        IL_007c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
        IL_0081:  ldloc.s    V_4
        IL_0083:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0088:  nop
        IL_0089:  leave.s    IL_009f

      }  // end handler
      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.s   -2
      IL_008e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
      IL_0093:  ldarg.0
      IL_0094:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
      IL_0099:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_009e:  nop
      IL_009f:  ret
    } // end of method '<AwaitDefaultHopToThreadPool>d__5'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<AwaitDefaultHopToThreadPool>d__5'::SetStateMachine

  } // end of class '<AwaitDefaultHopToThreadPool>d__5'

  .class auto ansi sealed nested private beforefieldinit '<SimpleVoidTaskMethod>d__6'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<SimpleVoidTaskMethod>d__6'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       190 (0xbe)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6' V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_0060

        IL_000e:  nop
        IL_000f:  ldstr      "Before"
        IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0019:  nop
        IL_001a:  ldc.r8     1.
        IL_0023:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_0028:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_002d:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0032:  stloc.1
        IL_0033:  ldloca.s   V_1
        IL_0035:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_003a:  brtrue.s   IL_007c

        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.0
        IL_003e:  dup
        IL_003f:  stloc.0
        IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
        IL_0045:  ldarg.0
        IL_0046:  ldloc.1
        IL_0047:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>u__1'
        IL_004c:  ldarg.0
        IL_004d:  stloc.2
        IL_004e:  ldarg.0
        IL_004f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
        IL_0054:  ldloca.s   V_1
        IL_0056:  ldloca.s   V_2
        IL_0058:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'>(!!0&,
                                                                                                                                                                                                                                                                                            !!1&)
        IL_005d:  nop
        IL_005e:  leave.s    IL_00bd

        IL_0060:  ldarg.0
        IL_0061:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>u__1'
        IL_0066:  stloc.1
        IL_0067:  ldarg.0
        IL_0068:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>u__1'
        IL_006d:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.m1
        IL_0075:  dup
        IL_0076:  stloc.0
        IL_0077:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
        IL_007c:  ldloca.s   V_1
        IL_007e:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0083:  nop
        IL_0084:  ldstr      "After"
        IL_0089:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_008e:  nop
        IL_008f:  leave.s    IL_00a9

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0091:  stloc.3
        IL_0092:  ldarg.0
        IL_0093:  ldc.i4.s   -2
        IL_0095:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
        IL_009a:  ldarg.0
        IL_009b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
        IL_00a0:  ldloc.3
        IL_00a1:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_00a6:  nop
        IL_00a7:  leave.s    IL_00bd

      }  // end handler
      IL_00a9:  ldarg.0
      IL_00aa:  ldc.i4.s   -2
      IL_00ac:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
      IL_00b1:  ldarg.0
      IL_00b2:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
      IL_00b7:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_00bc:  nop
      IL_00bd:  ret
    } // end of method '<SimpleVoidTaskMethod>d__6'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleVoidTaskMethod>d__6'::SetStateMachine

  } // end of class '<SimpleVoidTaskMethod>d__6'

  .class auto ansi sealed nested private beforefieldinit '<TaskMethodWithoutAwait>d__7'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<TaskMethodWithoutAwait>d__7'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       66 (0x42)
      .maxstack  2
      .locals init (int32 V_0,
               class [mscorlib]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  nop
        IL_0008:  ldstr      "No Await"
        IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0012:  nop
        IL_0013:  leave.s    IL_002d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0015:  stloc.1
        IL_0016:  ldarg.0
        IL_0017:  ldc.i4.s   -2
        IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
        IL_001e:  ldarg.0
        IL_001f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
        IL_0024:  ldloc.1
        IL_0025:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_002a:  nop
        IL_002b:  leave.s    IL_0041

      }  // end handler
      IL_002d:  ldarg.0
      IL_002e:  ldc.i4.s   -2
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
      IL_0035:  ldarg.0
      IL_0036:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
      IL_003b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_0040:  nop
      IL_0041:  ret
    } // end of method '<TaskMethodWithoutAwait>d__7'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<TaskMethodWithoutAwait>d__7'::SetStateMachine

  } // end of class '<TaskMethodWithoutAwait>d__7'

  .class auto ansi sealed nested private beforefieldinit '<SimpleBoolTaskMethod>d__8'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<SimpleBoolTaskMethod>d__8'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       195 (0xc3)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_0060

        IL_000e:  nop
        IL_000f:  ldstr      "Before"
        IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0019:  nop
        IL_001a:  ldc.r8     1.
        IL_0023:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_0028:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_002d:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0032:  stloc.2
        IL_0033:  ldloca.s   V_2
        IL_0035:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_003a:  brtrue.s   IL_007c

        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.0
        IL_003e:  dup
        IL_003f:  stloc.0
        IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
        IL_0045:  ldarg.0
        IL_0046:  ldloc.2
        IL_0047:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>u__1'
        IL_004c:  ldarg.0
        IL_004d:  stloc.3
        IL_004e:  ldarg.0
        IL_004f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
        IL_0054:  ldloca.s   V_2
        IL_0056:  ldloca.s   V_3
        IL_0058:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'>(!!0&,
                                                                                                                                                                                                                                                                                                              !!1&)
        IL_005d:  nop
        IL_005e:  leave.s    IL_00c2

        IL_0060:  ldarg.0
        IL_0061:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>u__1'
        IL_0066:  stloc.2
        IL_0067:  ldarg.0
        IL_0068:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>u__1'
        IL_006d:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.m1
        IL_0075:  dup
        IL_0076:  stloc.0
        IL_0077:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
        IL_007c:  ldloca.s   V_2
        IL_007e:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0083:  nop
        IL_0084:  ldstr      "After"
        IL_0089:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_008e:  nop
        IL_008f:  ldc.i4.1
        IL_0090:  stloc.1
        IL_0091:  leave.s    IL_00ad

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0093:  stloc.s    V_4
        IL_0095:  ldarg.0
        IL_0096:  ldc.i4.s   -2
        IL_0098:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
        IL_009d:  ldarg.0
        IL_009e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
        IL_00a3:  ldloc.s    V_4
        IL_00a5:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetException(class [mscorlib]System.Exception)
        IL_00aa:  nop
        IL_00ab:  leave.s    IL_00c2

      }  // end handler
      IL_00ad:  ldarg.0
      IL_00ae:  ldc.i4.s   -2
      IL_00b0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
      IL_00b5:  ldarg.0
      IL_00b6:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
      IL_00bb:  ldloc.1
      IL_00bc:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetResult(!0)
      IL_00c1:  nop
      IL_00c2:  ret
    } // end of method '<SimpleBoolTaskMethod>d__8'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleBoolTaskMethod>d__8'::SetStateMachine

  } // end of class '<SimpleBoolTaskMethod>d__8'

  .class auto ansi sealed nested private beforefieldinit '<TwoAwaitsWithDifferentAwaiterTypes>d__9'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private bool '<>s__1'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__1'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__2'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__9'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       324 (0x144)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9' V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_4,
               class [mscorlib]System.Exception V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.0
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0019

        IL_0012:  br.s       IL_0066

        IL_0014:  br         IL_00e2

        IL_0019:  nop
        IL_001a:  ldstr      "Before"
        IL_001f:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0024:  nop
        IL_0025:  ldarg.0
        IL_0026:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>4__this'
        IL_002b:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async::SimpleBoolTaskMethod()
        IL_0030:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
        IL_0035:  stloc.2
        IL_0036:  ldloca.s   V_2
        IL_0038:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
        IL_003d:  brtrue.s   IL_0082

        IL_003f:  ldarg.0
        IL_0040:  ldc.i4.0
        IL_0041:  dup
        IL_0042:  stloc.0
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_0048:  ldarg.0
        IL_0049:  ldloc.2
        IL_004a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__1'
        IL_004f:  ldarg.0
        IL_0050:  stloc.3
        IL_0051:  ldarg.0
        IL_0052:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
        IL_0057:  ldloca.s   V_2
        IL_0059:  ldloca.s   V_3
        IL_005b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'>(!!0&,
                                                                                                                                                                                                                                                                                                                  !!1&)
        IL_0060:  nop
        IL_0061:  leave      IL_0143

        IL_0066:  ldarg.0
        IL_0067:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__1'
        IL_006c:  stloc.2
        IL_006d:  ldarg.0
        IL_006e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__1'
        IL_0073:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_0079:  ldarg.0
        IL_007a:  ldc.i4.m1
        IL_007b:  dup
        IL_007c:  stloc.0
        IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_0082:  ldarg.0
        IL_0083:  ldloca.s   V_2
        IL_0085:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
        IL_008a:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>s__1'
        IL_008f:  ldarg.0
        IL_0090:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>s__1'
        IL_0095:  stloc.1
        IL_0096:  ldloc.1
        IL_0097:  brfalse.s  IL_0108

        IL_0099:  nop
        IL_009a:  ldc.r8     1.
        IL_00a3:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_00a8:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_00ad:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_00b2:  stloc.s    V_4
        IL_00b4:  ldloca.s   V_4
        IL_00b6:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_00bb:  brtrue.s   IL_00ff

        IL_00bd:  ldarg.0
        IL_00be:  ldc.i4.1
        IL_00bf:  dup
        IL_00c0:  stloc.0
        IL_00c1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_00c6:  ldarg.0
        IL_00c7:  ldloc.s    V_4
        IL_00c9:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__2'
        IL_00ce:  ldarg.0
        IL_00cf:  stloc.3
        IL_00d0:  ldarg.0
        IL_00d1:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
        IL_00d6:  ldloca.s   V_4
        IL_00d8:  ldloca.s   V_3
        IL_00da:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'>(!!0&,
                                                                                                                                                                                                                                                                                                          !!1&)
        IL_00df:  nop
        IL_00e0:  leave.s    IL_0143

        IL_00e2:  ldarg.0
        IL_00e3:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__2'
        IL_00e8:  stloc.s    V_4
        IL_00ea:  ldarg.0
        IL_00eb:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__2'
        IL_00f0:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_00f6:  ldarg.0
        IL_00f7:  ldc.i4.m1
        IL_00f8:  dup
        IL_00f9:  stloc.0
        IL_00fa:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_00ff:  ldloca.s   V_4
        IL_0101:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0106:  nop
        IL_0107:  nop
        IL_0108:  ldstr      "After"
        IL_010d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0112:  nop
        IL_0113:  leave.s    IL_012f

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0115:  stloc.s    V_5
        IL_0117:  ldarg.0
        IL_0118:  ldc.i4.s   -2
        IL_011a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_011f:  ldarg.0
        IL_0120:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
        IL_0125:  ldloc.s    V_5
        IL_0127:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_012c:  nop
        IL_012d:  leave.s    IL_0143

      }  // end handler
      IL_012f:  ldarg.0
      IL_0130:  ldc.i4.s   -2
      IL_0132:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
      IL_0137:  ldarg.0
      IL_0138:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
      IL_013d:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0142:  nop
      IL_0143:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__9'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__9'::SetStateMachine

  } // end of class '<TwoAwaitsWithDifferentAwaiterTypes>d__9'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInLoopCondition>d__10'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private bool '<>s__1'
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
    } // end of method '<AwaitInLoopCondition>d__10'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       192 (0xc0)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_005c

        IL_000e:  nop
        IL_000f:  br.s       IL_001e

        IL_0011:  nop
        IL_0012:  ldstr      "Body"
        IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_001c:  nop
        IL_001d:  nop
        IL_001e:  ldarg.0
        IL_001f:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>4__this'
        IL_0024:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async::SimpleBoolTaskMethod()
        IL_0029:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
        IL_002e:  stloc.2
        IL_002f:  ldloca.s   V_2
        IL_0031:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
        IL_0036:  brtrue.s   IL_0078

        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.0
        IL_003a:  dup
        IL_003b:  stloc.0
        IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
        IL_0041:  ldarg.0
        IL_0042:  ldloc.2
        IL_0043:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>u__1'
        IL_0048:  ldarg.0
        IL_0049:  stloc.3
        IL_004a:  ldarg.0
        IL_004b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
        IL_0050:  ldloca.s   V_2
        IL_0052:  ldloca.s   V_3
        IL_0054:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'>(!!0&,
                                                                                                                                                                                                                                                                                                     !!1&)
        IL_0059:  nop
        IL_005a:  leave.s    IL_00bf

        IL_005c:  ldarg.0
        IL_005d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>u__1'
        IL_0062:  stloc.2
        IL_0063:  ldarg.0
        IL_0064:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>u__1'
        IL_0069:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_006f:  ldarg.0
        IL_0070:  ldc.i4.m1
        IL_0071:  dup
        IL_0072:  stloc.0
        IL_0073:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
        IL_0078:  ldarg.0
        IL_0079:  ldloca.s   V_2
        IL_007b:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
        IL_0080:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>s__1'
        IL_0085:  ldarg.0
        IL_0086:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>s__1'
        IL_008b:  stloc.1
        IL_008c:  ldloc.1
        IL_008d:  brtrue.s   IL_0011

        IL_008f:  leave.s    IL_00ab

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0091:  stloc.s    V_4
        IL_0093:  ldarg.0
        IL_0094:  ldc.i4.s   -2
        IL_0096:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
        IL_009b:  ldarg.0
        IL_009c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
        IL_00a1:  ldloc.s    V_4
        IL_00a3:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_00a8:  nop
        IL_00a9:  leave.s    IL_00bf

      }  // end handler
      IL_00ab:  ldarg.0
      IL_00ac:  ldc.i4.s   -2
      IL_00ae:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
      IL_00b3:  ldarg.0
      IL_00b4:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
      IL_00b9:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00be:  nop
      IL_00bf:  ret
    } // end of method '<AwaitInLoopCondition>d__10'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<AwaitInLoopCondition>d__10'::SetStateMachine

  } // end of class '<AwaitInLoopCondition>d__10'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInCatch>d__11'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public bool b
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task1
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task2
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private object '<>s__1'
    .field private int32 '<>s__2'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<AwaitInCatch>d__11'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       386 (0x182)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11' V_2,
               class [mscorlib]System.Exception V_3,
               int32 V_4,
               bool V_5,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_6,
               class [mscorlib]System.Exception V_7)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.0
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0019

        IL_0012:  br.s       IL_0021

        IL_0014:  br         IL_0112

        IL_0019:  nop
        IL_001a:  ldarg.0
        IL_001b:  ldc.i4.0
        IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>s__2'
        IL_0021:  nop
        .try
        {
          IL_0022:  ldloc.0
          IL_0023:  brfalse.s  IL_0027

          IL_0025:  br.s       IL_0029

          IL_0027:  br.s       IL_0071

          IL_0029:  nop
          IL_002a:  ldstr      "Start try"
          IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0034:  nop
          IL_0035:  ldarg.0
          IL_0036:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task1
          IL_003b:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
          IL_0040:  stloc.1
          IL_0041:  ldloca.s   V_1
          IL_0043:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
          IL_0048:  brtrue.s   IL_008d

          IL_004a:  ldarg.0
          IL_004b:  ldc.i4.0
          IL_004c:  dup
          IL_004d:  stloc.0
          IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
          IL_0053:  ldarg.0
          IL_0054:  ldloc.1
          IL_0055:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
          IL_005a:  ldarg.0
          IL_005b:  stloc.2
          IL_005c:  ldarg.0
          IL_005d:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
          IL_0062:  ldloca.s   V_1
          IL_0064:  ldloca.s   V_2
          IL_0066:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'>(!!0&,
                                                                                                                                                                                                                                                                                                !!1&)
          IL_006b:  nop
          IL_006c:  leave      IL_0181

          IL_0071:  ldarg.0
          IL_0072:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
          IL_0077:  stloc.1
          IL_0078:  ldarg.0
          IL_0079:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
          IL_007e:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
          IL_0084:  ldarg.0
          IL_0085:  ldc.i4.m1
          IL_0086:  dup
          IL_0087:  stloc.0
          IL_0088:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
          IL_008d:  ldloca.s   V_1
          IL_008f:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
          IL_0094:  pop
          IL_0095:  ldstr      "End try"
          IL_009a:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_009f:  nop
          IL_00a0:  nop
          IL_00a1:  leave.s    IL_00b4

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_00a3:  stloc.3
          IL_00a4:  ldarg.0
          IL_00a5:  ldloc.3
          IL_00a6:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>s__1'
          IL_00ab:  ldarg.0
          IL_00ac:  ldc.i4.1
          IL_00ad:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>s__2'
          IL_00b2:  leave.s    IL_00b4

        }  // end handler
        IL_00b4:  ldarg.0
        IL_00b5:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>s__2'
        IL_00ba:  stloc.s    V_4
        IL_00bc:  ldloc.s    V_4
        IL_00be:  ldc.i4.1
        IL_00bf:  beq.s      IL_00c6

        IL_00c1:  br         IL_014a

        IL_00c6:  nop
        IL_00c7:  ldarg.0
        IL_00c8:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::b
        IL_00cd:  ldc.i4.0
        IL_00ce:  ceq
        IL_00d0:  stloc.s    V_5
        IL_00d2:  ldloc.s    V_5
        IL_00d4:  brfalse.s  IL_013a

        IL_00d6:  nop
        IL_00d7:  ldarg.0
        IL_00d8:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task2
        IL_00dd:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
        IL_00e2:  stloc.s    V_6
        IL_00e4:  ldloca.s   V_6
        IL_00e6:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
        IL_00eb:  brtrue.s   IL_012f

        IL_00ed:  ldarg.0
        IL_00ee:  ldc.i4.1
        IL_00ef:  dup
        IL_00f0:  stloc.0
        IL_00f1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
        IL_00f6:  ldarg.0
        IL_00f7:  ldloc.s    V_6
        IL_00f9:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
        IL_00fe:  ldarg.0
        IL_00ff:  stloc.2
        IL_0100:  ldarg.0
        IL_0101:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
        IL_0106:  ldloca.s   V_6
        IL_0108:  ldloca.s   V_2
        IL_010a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'>(!!0&,
                                                                                                                                                                                                                                                                                              !!1&)
        IL_010f:  nop
        IL_0110:  leave.s    IL_0181

        IL_0112:  ldarg.0
        IL_0113:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
        IL_0118:  stloc.s    V_6
        IL_011a:  ldarg.0
        IL_011b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
        IL_0120:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
        IL_0126:  ldarg.0
        IL_0127:  ldc.i4.m1
        IL_0128:  dup
        IL_0129:  stloc.0
        IL_012a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
        IL_012f:  ldloca.s   V_6
        IL_0131:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
        IL_0136:  pop
        IL_0137:  nop
        IL_0138:  br.s       IL_0147

        IL_013a:  nop
        IL_013b:  ldstr      "No await"
        IL_0140:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0145:  nop
        IL_0146:  nop
        IL_0147:  nop
        IL_0148:  br.s       IL_014a

        IL_014a:  ldarg.0
        IL_014b:  ldnull
        IL_014c:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>s__1'
        IL_0151:  leave.s    IL_016d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0153:  stloc.s    V_7
        IL_0155:  ldarg.0
        IL_0156:  ldc.i4.s   -2
        IL_0158:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
        IL_015d:  ldarg.0
        IL_015e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
        IL_0163:  ldloc.s    V_7
        IL_0165:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_016a:  nop
        IL_016b:  leave.s    IL_0181

      }  // end handler
      IL_016d:  ldarg.0
      IL_016e:  ldc.i4.s   -2
      IL_0170:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
      IL_0175:  ldarg.0
      IL_0176:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
      IL_017b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_0180:  nop
      IL_0181:  ret
    } // end of method '<AwaitInCatch>d__11'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<AwaitInCatch>d__11'::SetStateMachine

  } // end of class '<AwaitInCatch>d__11'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInFinally>d__12'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public bool b
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task1
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task2
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private object '<>s__1'
    .field private int32 '<>s__2'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<AwaitInFinally>d__12'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       413 (0x19d)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12' V_2,
               object V_3,
               bool V_4,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_5,
               class [mscorlib]System.Exception V_6)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.0
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0019

        IL_0012:  br.s       IL_0028

        IL_0014:  br         IL_0103

        IL_0019:  nop
        IL_001a:  ldarg.0
        IL_001b:  ldnull
        IL_001c:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>s__1'
        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.0
        IL_0023:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>s__2'
        IL_0028:  nop
        .try
        {
          IL_0029:  ldloc.0
          IL_002a:  brfalse.s  IL_002e

          IL_002c:  br.s       IL_0030

          IL_002e:  br.s       IL_0078

          IL_0030:  nop
          IL_0031:  ldstr      "Start try"
          IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_003b:  nop
          IL_003c:  ldarg.0
          IL_003d:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task1
          IL_0042:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
          IL_0047:  stloc.1
          IL_0048:  ldloca.s   V_1
          IL_004a:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
          IL_004f:  brtrue.s   IL_0094

          IL_0051:  ldarg.0
          IL_0052:  ldc.i4.0
          IL_0053:  dup
          IL_0054:  stloc.0
          IL_0055:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
          IL_005a:  ldarg.0
          IL_005b:  ldloc.1
          IL_005c:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
          IL_0061:  ldarg.0
          IL_0062:  stloc.2
          IL_0063:  ldarg.0
          IL_0064:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
          IL_0069:  ldloca.s   V_1
          IL_006b:  ldloca.s   V_2
          IL_006d:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'>(!!0&,
                                                                                                                                                                                                                                                                                                  !!1&)
          IL_0072:  nop
          IL_0073:  leave      IL_019c

          IL_0078:  ldarg.0
          IL_0079:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
          IL_007e:  stloc.1
          IL_007f:  ldarg.0
          IL_0080:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
          IL_0085:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
          IL_008b:  ldarg.0
          IL_008c:  ldc.i4.m1
          IL_008d:  dup
          IL_008e:  stloc.0
          IL_008f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
          IL_0094:  ldloca.s   V_1
          IL_0096:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
          IL_009b:  pop
          IL_009c:  ldstr      "End try"
          IL_00a1:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00a6:  nop
          IL_00a7:  nop
          IL_00a8:  leave.s    IL_00b4

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_00aa:  stloc.3
          IL_00ab:  ldarg.0
          IL_00ac:  ldloc.3
          IL_00ad:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>s__1'
          IL_00b2:  leave.s    IL_00b4

        }  // end handler
        IL_00b4:  nop
        IL_00b5:  ldarg.0
        IL_00b6:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::b
        IL_00bb:  ldc.i4.0
        IL_00bc:  ceq
        IL_00be:  stloc.s    V_4
        IL_00c0:  ldloc.s    V_4
        IL_00c2:  brfalse.s  IL_012b

        IL_00c4:  nop
        IL_00c5:  ldarg.0
        IL_00c6:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task2
        IL_00cb:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
        IL_00d0:  stloc.s    V_5
        IL_00d2:  ldloca.s   V_5
        IL_00d4:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
        IL_00d9:  brtrue.s   IL_0120

        IL_00db:  ldarg.0
        IL_00dc:  ldc.i4.1
        IL_00dd:  dup
        IL_00de:  stloc.0
        IL_00df:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
        IL_00e4:  ldarg.0
        IL_00e5:  ldloc.s    V_5
        IL_00e7:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
        IL_00ec:  ldarg.0
        IL_00ed:  stloc.2
        IL_00ee:  ldarg.0
        IL_00ef:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
        IL_00f4:  ldloca.s   V_5
        IL_00f6:  ldloca.s   V_2
        IL_00f8:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'>(!!0&,
                                                                                                                                                                                                                                                                                                !!1&)
        IL_00fd:  nop
        IL_00fe:  leave      IL_019c

        IL_0103:  ldarg.0
        IL_0104:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
        IL_0109:  stloc.s    V_5
        IL_010b:  ldarg.0
        IL_010c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
        IL_0111:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
        IL_0117:  ldarg.0
        IL_0118:  ldc.i4.m1
        IL_0119:  dup
        IL_011a:  stloc.0
        IL_011b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
        IL_0120:  ldloca.s   V_5
        IL_0122:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
        IL_0127:  pop
        IL_0128:  nop
        IL_0129:  br.s       IL_0138

        IL_012b:  nop
        IL_012c:  ldstr      "No await"
        IL_0131:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0136:  nop
        IL_0137:  nop
        IL_0138:  nop
        IL_0139:  ldarg.0
        IL_013a:  ldfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>s__1'
        IL_013f:  stloc.3
        IL_0140:  ldloc.3
        IL_0141:  brfalse.s  IL_015e

        IL_0143:  ldloc.3
        IL_0144:  isinst     [mscorlib]System.Exception
        IL_0149:  stloc.s    V_6
        IL_014b:  ldloc.s    V_6
        IL_014d:  brtrue.s   IL_0151

        IL_014f:  ldloc.3
        IL_0150:  throw

        IL_0151:  ldloc.s    V_6
        IL_0153:  call       class [mscorlib]System.Runtime.ExceptionServices.ExceptionDispatchInfo [mscorlib]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Capture(class [mscorlib]System.Exception)
        IL_0158:  callvirt   instance void [mscorlib]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Throw()
        IL_015d:  nop
        IL_015e:  ldarg.0
        IL_015f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>s__2'
        IL_0164:  pop
        IL_0165:  ldarg.0
        IL_0166:  ldnull
        IL_0167:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>s__1'
        IL_016c:  leave.s    IL_0188

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_016e:  stloc.s    V_6
        IL_0170:  ldarg.0
        IL_0171:  ldc.i4.s   -2
        IL_0173:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
        IL_0178:  ldarg.0
        IL_0179:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
        IL_017e:  ldloc.s    V_6
        IL_0180:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0185:  nop
        IL_0186:  leave.s    IL_019c

      }  // end handler
      IL_0188:  ldarg.0
      IL_0189:  ldc.i4.s   -2
      IL_018b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
      IL_0190:  ldarg.0
      IL_0191:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
      IL_0196:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_019b:  nop
      IL_019c:  ret
    } // end of method '<AwaitInFinally>d__12'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'>(!!0&)
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
                                                                                                                                       5F 5F 31 00 00 )                                  // __1..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'>(!!0&)
    IL_002f:  ret
  } // end of method Async::VoidMethodWithoutAwait

  .method public hidebysig instance void 
          EmptyVoidMethod() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 49 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..IICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 45 6D 70 74 79 56 6F 69   // .Async+<EmptyVoi
                                                                                                                                       64 4D 65 74 68 6F 64 3E 64 5F 5F 32 00 00 )       // dMethod>d__2..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'>(!!0&)
    IL_002f:  ret
  } // end of method Async::EmptyVoidMethod

  .method public hidebysig instance void 
          AwaitYield() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 44 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..DICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 59 69 65   // .Async+<AwaitYie
                                                                                                                                       6C 64 3E 64 5F 5F 33 00 00 )                      // ld>d__3..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'>(!!0&)
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
                                                                                                                                       6C 65 3E 64 5F 5F 34 00 00 )                      // le>d__4..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'>(!!0&)
    IL_002f:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'>(!!0&)
    IL_002f:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'>(!!0&)
    IL_002f:  ldloc.0
    IL_0030:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
    IL_0035:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_003a:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'>(!!0&)
    IL_002f:  ldloc.0
    IL_0030:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
    IL_0035:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_003a:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'>(!!0&)
    IL_002f:  ldloc.0
    IL_0030:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
    IL_0035:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::get_Task()
    IL_003a:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'>(!!0&)
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
                                                                                                                                       31 30 00 00 )                                     // 10..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'>(!!0&)
    IL_002f:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::b
    IL_0014:  ldloc.0
    IL_0015:  ldarg.2
    IL_0016:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task1
    IL_001b:  ldloc.0
    IL_001c:  ldarg.3
    IL_001d:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task2
    IL_0022:  ldloc.0
    IL_0023:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0028:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.m1
    IL_002f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
    IL_0034:  ldloc.0
    IL_0035:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
    IL_003a:  stloc.1
    IL_003b:  ldloca.s   V_1
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'>(!!0&)
    IL_0044:  ldloc.0
    IL_0045:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
    IL_004a:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_004f:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::b
    IL_0014:  ldloc.0
    IL_0015:  ldarg.2
    IL_0016:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task1
    IL_001b:  ldloc.0
    IL_001c:  ldarg.3
    IL_001d:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task2
    IL_0022:  ldloc.0
    IL_0023:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0028:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.m1
    IL_002f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
    IL_0034:  ldloc.0
    IL_0035:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
    IL_003a:  stloc.1
    IL_003b:  ldloca.s   V_1
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'>(!!0&)
    IL_0044:  ldloc.0
    IL_0045:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
    IL_004a:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_004f:  ret
  } // end of method Async::AwaitInFinally

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Async::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
       extends [mscorlib]System.ValueType
       implements [mscorlib]System.Runtime.CompilerServices.INotifyCompletion
{
  .field private bool '<IsCompleted>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method HopToThreadPoolAwaitable::GetAwaiter

  .method public hidebysig newslot virtual final 
          instance void  OnCompleted(class [mscorlib]System.Action continuation) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Run(class [mscorlib]System.Action)
    IL_0007:  pop
    IL_0008:  ret
  } // end of method HopToThreadPoolAwaitable::OnCompleted

  .method public hidebysig instance void 
          GetResult() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method HopToThreadPoolAwaitable::GetResult

  .property instance bool IsCompleted()
  {
    .get instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::get_IsCompleted()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::set_IsCompleted(bool)
  } // end of property HopToThreadPoolAwaitable::IsCompleted
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
