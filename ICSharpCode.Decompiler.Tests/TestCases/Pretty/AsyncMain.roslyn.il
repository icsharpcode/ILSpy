



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public string[] args
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
    } // end of method '<Main>d__0'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       170 (0xaa)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0' V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_004c

        IL_000e:  nop
        IL_000f:  ldc.i4     0x3e8
        IL_0014:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(int32)
        IL_0019:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_001e:  stloc.1
        IL_001f:  ldloca.s   V_1
        IL_0021:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_0026:  brtrue.s   IL_0068

        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.0
        IL_002a:  dup
        IL_002b:  stloc.0
        IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
        IL_0031:  ldarg.0
        IL_0032:  ldloc.1
        IL_0033:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>u__1'
        IL_0038:  ldarg.0
        IL_0039:  stloc.2
        IL_003a:  ldarg.0
        IL_003b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
        IL_0040:  ldloca.s   V_1
        IL_0042:  ldloca.s   V_2
        IL_0044:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                !!1&)
        IL_0049:  nop
        IL_004a:  leave.s    IL_00a9

        IL_004c:  ldarg.0
        IL_004d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>u__1'
        IL_0052:  stloc.1
        IL_0053:  ldarg.0
        IL_0054:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>u__1'
        IL_0059:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_005f:  ldarg.0
        IL_0060:  ldc.i4.m1
        IL_0061:  dup
        IL_0062:  stloc.0
        IL_0063:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
        IL_0068:  ldloca.s   V_1
        IL_006a:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_006f:  nop
        IL_0070:  ldstr      "Hello Wolrd!"
        IL_0075:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_007a:  nop
        IL_007b:  leave.s    IL_0095

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_007d:  stloc.3
        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.s   -2
        IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
        IL_0086:  ldarg.0
        IL_0087:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
        IL_008c:  ldloc.3
        IL_008d:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0092:  nop
        IL_0093:  leave.s    IL_00a9

      }  // end handler
      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.s   -2
      IL_0098:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
      IL_009d:  ldarg.0
      IL_009e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
      IL_00a3:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_00a8:  nop
      IL_00a9:  ret
    } // end of method '<Main>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      string[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::args
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'>(!!0&)
    IL_002f:  ldloc.0
    IL_0030:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.AsyncMain/'<Main>d__0'::'<>t__builder'
    IL_0035:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_003a:  ret
  } // end of method AsyncMain::Main

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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
