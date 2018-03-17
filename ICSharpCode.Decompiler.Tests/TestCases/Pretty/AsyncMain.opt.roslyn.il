



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly AsyncMain
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
.module AsyncMain.exe
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<Main>d__0'
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
      // Code size       157 (0x9d)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_1,
               class [mscorlib]System.Exception V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0043

        IL_000a:  ldc.i4     0x3e8
        IL_000f:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(int32)
        IL_0014:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0019:  stloc.1
        IL_001a:  ldloca.s   V_1
        IL_001c:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_0021:  brtrue.s   IL_005f

        IL_0023:  ldarg.0
        IL_0024:  ldc.i4.0
        IL_0025:  dup
        IL_0026:  stloc.0
        IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
        IL_002c:  ldarg.0
        IL_002d:  ldloc.1
        IL_002e:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>u__1'
        IL_0033:  ldarg.0
        IL_0034:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
        IL_0039:  ldloca.s   V_1
        IL_003b:  ldarg.0
        IL_003c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                    !!1&)
        IL_0041:  leave.s    IL_009c

        IL_0043:  ldarg.0
        IL_0044:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>u__1'
        IL_0049:  stloc.1
        IL_004a:  ldarg.0
        IL_004b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>u__1'
        IL_0050:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.m1
        IL_0058:  dup
        IL_0059:  stloc.0
        IL_005a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
        IL_005f:  ldloca.s   V_1
        IL_0061:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0066:  ldstr      "Hello Wolrd!"
        IL_006b:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0070:  leave.s    IL_0089

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0072:  stloc.2
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.s   -2
        IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
        IL_007b:  ldarg.0
        IL_007c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
        IL_0081:  ldloc.2
        IL_0082:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0087:  leave.s    IL_009c

      }  // end handler
      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.s   -2
      IL_008c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
      IL_0091:  ldarg.0
      IL_0092:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
      IL_0097:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_009c:  ret
    } // end of method '<Main>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<Main>d__0'::SetStateMachine

  } // end of class '<Main>d__0'

  .method public hidebysig static class [mscorlib]System.Threading.Tasks.Task 
          Main(string[] args) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 42 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..BICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 4D 61 69 6E 2B 3C 4D 61 69 6E   // .AsyncMain+<Main
                                                                                                                                       3E 64 5F 5F 30 00 00 )                            // >d__0..
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'>(!!0&)
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
    IL_002b:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0030:  ret
  } // end of method AsyncMain::Main

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method AsyncMain::.ctor

  .method private hidebysig specialname static 
          void  '<Main>'(string[] args) cil managed
  {
    .entrypoint
    // Code size       20 (0x14)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       class [mscorlib]System.Threading.Tasks.Task ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain::Main(string[])
    IL_0006:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
    IL_000b:  stloc.0
    IL_000c:  ldloca.s   V_0
    IL_000e:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
    IL_0013:  ret
  } // end of method AsyncMain::'<Main>'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
