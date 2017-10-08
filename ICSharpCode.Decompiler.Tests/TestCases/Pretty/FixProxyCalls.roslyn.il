
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly FixProxyCalls
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
.module FixProxyCalls.dll
// MVID: {33B12B9F-34E5-4EDD-B8C4-0A47397C1BC1}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01280000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__0'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> '<>t__builder'
    .field public string test
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler '<>4__this'
    .field private string '<>s__1'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<Test>d__0'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       181 (0xb5)
      .maxstack  3
      .locals init (int32 V_0,
               string V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_0053

        IL_000e:  nop
        IL_000f:  ldarg.0
        IL_0010:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>4__this'
        IL_0015:  ldarg.0
        IL_0016:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::test
        IL_001b:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler::'<>n__0'(string)
        IL_0020:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<string>::GetAwaiter()
        IL_0025:  stloc.2
        IL_0026:  ldloca.s   V_2
        IL_0028:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::get_IsCompleted()
        IL_002d:  brtrue.s   IL_006f

        IL_002f:  ldarg.0
        IL_0030:  ldc.i4.0
        IL_0031:  dup
        IL_0032:  stloc.0
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>1__state'
        IL_0038:  ldarg.0
        IL_0039:  ldloc.2
        IL_003a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>u__1'
        IL_003f:  ldarg.0
        IL_0040:  stloc.3
        IL_0041:  ldarg.0
        IL_0042:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>t__builder'
        IL_0047:  ldloca.s   V_2
        IL_0049:  ldloca.s   V_3
        IL_004b:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>,class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                                                       !!1&)
        IL_0050:  nop
        IL_0051:  leave.s    IL_00b4

        IL_0053:  ldarg.0
        IL_0054:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>u__1'
        IL_0059:  stloc.2
        IL_005a:  ldarg.0
        IL_005b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>u__1'
        IL_0060:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>
        IL_0066:  ldarg.0
        IL_0067:  ldc.i4.m1
        IL_0068:  dup
        IL_0069:  stloc.0
        IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>1__state'
        IL_006f:  ldarg.0
        IL_0070:  ldloca.s   V_2
        IL_0072:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::GetResult()
        IL_0077:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>s__1'
        IL_007c:  ldarg.0
        IL_007d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>s__1'
        IL_0082:  stloc.1
        IL_0083:  leave.s    IL_009f

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0085:  stloc.s    V_4
        IL_0087:  ldarg.0
        IL_0088:  ldc.i4.s   -2
        IL_008a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>1__state'
        IL_008f:  ldarg.0
        IL_0090:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>t__builder'
        IL_0095:  ldloc.s    V_4
        IL_0097:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetException(class [mscorlib]System.Exception)
        IL_009c:  nop
        IL_009d:  leave.s    IL_00b4

      }  // end handler
      IL_009f:  ldarg.0
      IL_00a0:  ldc.i4.s   -2
      IL_00a2:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>1__state'
      IL_00a7:  ldarg.0
      IL_00a8:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>t__builder'
      IL_00ad:  ldloc.1
      IL_00ae:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetResult(!0)
      IL_00b3:  nop
      IL_00b4:  ret
    } // end of method '<Test>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test>d__0'::SetStateMachine

  } // end of class '<Test>d__0'

  .method famorassem hidebysig virtual instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          Test(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..KICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                       74 79 2E 41 77 61 69 74 54 65 73 74 48 61 6E 64   // ty.AwaitTestHand
                                                                                                                                       6C 65 72 2B 3C 54 65 73 74 3E 64 5F 5F 30 00 00 ) // ler+<Test>d__0..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       66 (0x42)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::test
    IL_0014:  ldloc.0
    IL_0015:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Create()
    IL_001a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>t__builder'
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.m1
    IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>1__state'
    IL_0026:  ldloc.0
    IL_0027:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>t__builder'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloca.s   V_0
    IL_0031:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Start<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'>(!!0&)
    IL_0036:  ldloc.0
    IL_0037:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__0'::'<>t__builder'
    IL_003c:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::get_Task()
    IL_0041:  ret
  } // end of method AwaitTestHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method AwaitTestHandler::.ctor

  .method private hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          '<>n__0'(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler::Test(string)
    IL_0007:  ret
  } // end of method AwaitTestHandler::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass0_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string test
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass0_0'::.ctor

    .method assembly hidebysig instance string 
            '<Test>b__0'() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass0_0'::test
      IL_0007:  callvirt   instance string [mscorlib]System.String::ToUpper()
      IL_000c:  stloc.0
      IL_000d:  br.s       IL_000f

      IL_000f:  ldloc.0
      IL_0010:  ret
    } // end of method '<>c__DisplayClass0_0'::'<Test>b__0'

  } // end of class '<>c__DisplayClass0_0'

  .method famorassem hidebysig newslot virtual 
          instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          Test(string test) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass0_0' V_0,
             class [mscorlib]System.Threading.Tasks.Task`1<string> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass0_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass0_0'::test
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass0_0'::'<Test>b__0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_001a:  call       class [mscorlib]System.Threading.Tasks.Task`1<!!0> [mscorlib]System.Threading.Tasks.Task::Run<string>(class [mscorlib]System.Func`1<!!0>)
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method AwaitDelegatingHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method AwaitDelegatingHandler::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler
{
  .class auto ansi sealed nested private beforefieldinit '<Test2>d__0'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<string>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private string '<>2__current'
    .field private int32 '<>l__initialThreadId'
    .field private string test
    .field public string '<>3__test'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler '<>4__this'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 '<>1__state') cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<Test2>d__0'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test2>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       73 (0x49)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  br.s       IL_000c

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  beq.s      IL_0014

      IL_0010:  br.s       IL_0016

      IL_0012:  br.s       IL_0018

      IL_0014:  br.s       IL_0040

      IL_0016:  ldc.i4.0
      IL_0017:  ret

      IL_0018:  ldarg.0
      IL_0019:  ldc.i4.m1
      IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_001f:  nop
      IL_0020:  ldarg.0
      IL_0021:  ldarg.0
      IL_0022:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
      IL_0027:  ldarg.0
      IL_0028:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::test
      IL_002d:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler::'<>n__0'(string)
      IL_0032:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.m1
      IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0047:  ldc.i4.0
      IL_0048:  ret
    } // end of method '<Test2>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>2__current'
      IL_0006:  ret
    } // end of method '<Test2>d__0'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<Test2>d__0'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>2__current'
      IL_0006:  ret
    } // end of method '<Test2>d__0'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
      IL_0030:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>3__test'
      IL_003c:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::test
      IL_0041:  ldloc.0
      IL_0042:  ret
    } // end of method '<Test2>d__0'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<Test2>d__0'::System.Collections.IEnumerable.GetEnumerator

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<Test2>d__0'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::System.Collections.IEnumerator.get_Current()
    } // end of property '<Test2>d__0'::System.Collections.IEnumerator.Current
  } // end of class '<Test2>d__0'

  .method famorassem hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          Test2(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..LICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                          74 79 2E 59 69 65 6C 64 54 65 73 74 48 61 6E 64   // ty.YieldTestHand
                                                                                                                                          6C 65 72 2B 3C 54 65 73 74 32 3E 64 5F 5F 30 00   // ler+<Test2>d__0.
                                                                                                                                          00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
    IL_000e:  dup
    IL_000f:  ldarg.1
    IL_0010:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>3__test'
    IL_0015:  ret
  } // end of method YieldTestHandler::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method YieldTestHandler::.ctor

  .method private hidebysig instance string 
          '<>n__0'(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler::Test(string)
    IL_0007:  ret
  } // end of method YieldTestHandler::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance string  Test(string test) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  newarr     [mscorlib]System.String
    IL_0008:  dup
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "fsdf"
    IL_000f:  stelem.ref
    IL_0010:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0015:  stloc.0
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method YieldDelegatingHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method YieldDelegatingHandler::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass0_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string test
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler '<>4__this'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass0_0'::.ctor

    .method assembly hidebysig instance string 
            '<Test>b__0'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0'::'<>4__this'
      IL_0006:  ldarg.0
      IL_0007:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0'::test
      IL_000c:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler::'<>n__0'(string)
      IL_0011:  ret
    } // end of method '<>c__DisplayClass0_0'::'<Test>b__0'

  } // end of class '<>c__DisplayClass0_0'

  .method famorassem hidebysig virtual instance string 
          Test(string test) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0' V_0,
             class [mscorlib]System.Func`1<string> V_1,
             string V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0'::test
    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass0_0'::'<Test>b__0'()
    IL_001c:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  callvirt   instance !0 class [mscorlib]System.Func`1<string>::Invoke()
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.2
    IL_002c:  ret
  } // end of method LambdaHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method LambdaHandler::.ctor

  .method private hidebysig instance string 
          '<>n__0'(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler::Test(string)
    IL_0007:  ret
  } // end of method LambdaHandler::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance string  Test(string test) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  5
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  newarr     [mscorlib]System.String
    IL_0008:  dup
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "fsdf"
    IL_000f:  stelem.ref
    IL_0010:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0015:  stloc.0
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method LambdaDelegatingHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method LambdaDelegatingHandler::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
