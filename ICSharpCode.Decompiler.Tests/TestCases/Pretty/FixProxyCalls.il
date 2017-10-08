
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '3b3jnobf'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '3b3jnobf.dll'
// MVID: {49E27949-DBA3-4290-A606-8A8CBA8C564E}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00FC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string test
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1'::.ctor

    .method public hidebysig instance string 
            '<Test>b__0'() cil managed
    {
      // Code size       17 (0x11)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass1'::test
      IL_0007:  callvirt   instance string [mscorlib]System.String::ToUpper()
      IL_000c:  stloc.0
      IL_000d:  br.s       IL_000f

      IL_000f:  ldloc.0
      IL_0010:  ret
    } // end of method '<>c__DisplayClass1'::'<Test>b__0'

  } // end of class '<>c__DisplayClass1'

  .method famorassem hidebysig newslot virtual 
          instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          Test(string test) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass1' V_0,
             class [mscorlib]System.Threading.Tasks.Task`1<string> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass1'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass1'::test
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler/'<>c__DisplayClass1'::'<Test>b__0'()
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method AwaitDelegatingHandler::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__3'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler '<>4__this'
    .field public string test
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> '<>u__$awaiter4'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       184 (0xb8)
      .maxstack  3
      .locals init (bool V_0,
               string V_1,
               class [mscorlib]System.Exception V_2,
               int32 V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> V_4,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> V_5)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>1__state'
        IL_0008:  stloc.3
        IL_0009:  ldloc.3
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_0057

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  ldarg.0
        IL_0015:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>4__this'
        IL_001a:  ldarg.0
        IL_001b:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::test
        IL_0020:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler::'<>n__FabricatedMethod5'(string)
        IL_0025:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<string>::GetAwaiter()
        IL_002a:  stloc.s    V_4
        IL_002c:  ldloca.s   V_4
        IL_002e:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::get_IsCompleted()
        IL_0033:  brtrue.s   IL_0076

        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.0
        IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>1__state'
        IL_003c:  ldarg.0
        IL_003d:  ldloc.s    V_4
        IL_003f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>u__$awaiter4'
        IL_0044:  ldarg.0
        IL_0045:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
        IL_004a:  ldloca.s   V_4
        IL_004c:  ldarg.0
        IL_004d:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>,valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'>(!!0&,
                                                                                                                                                                                                                                                                                                                           !!1&)
        IL_0052:  nop
        IL_0053:  ldc.i4.0
        IL_0054:  stloc.0
        IL_0055:  leave.s    IL_00b6

        IL_0057:  ldarg.0
        IL_0058:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>u__$awaiter4'
        IL_005d:  stloc.s    V_4
        IL_005f:  ldarg.0
        IL_0060:  ldloca.s   V_5
        IL_0062:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>
        IL_0068:  ldloc.s    V_5
        IL_006a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>u__$awaiter4'
        IL_006f:  ldarg.0
        IL_0070:  ldc.i4.m1
        IL_0071:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>1__state'
        IL_0076:  ldloca.s   V_4
        IL_0078:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::GetResult()
        IL_007d:  ldloca.s   V_4
        IL_007f:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>
        IL_0085:  stloc.1
        IL_0086:  leave.s    IL_00a0

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0088:  stloc.2
        IL_0089:  ldarg.0
        IL_008a:  ldc.i4.s   -2
        IL_008c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>1__state'
        IL_0091:  ldarg.0
        IL_0092:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
        IL_0097:  ldloc.2
        IL_0098:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetException(class [mscorlib]System.Exception)
        IL_009d:  nop
        IL_009e:  leave.s    IL_00b6

      }  // end handler
      IL_00a0:  nop
      IL_00a1:  ldarg.0
      IL_00a2:  ldc.i4.s   -2
      IL_00a4:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>1__state'
      IL_00a9:  ldarg.0
      IL_00aa:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
      IL_00af:  ldloc.1
      IL_00b0:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetResult(!0)
      IL_00b5:  nop
      IL_00b6:  nop
      IL_00b7:  ret
    } // end of method '<Test>d__3'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<Test>d__3'::SetStateMachine

  } // end of class '<Test>d__3'

  .method famorassem hidebysig virtual instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          Test(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..KICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                       74 79 2E 41 77 61 69 74 54 65 73 74 48 61 6E 64   // ty.AwaitTestHand
                                                                                                                                       6C 65 72 2B 3C 54 65 73 74 3E 64 5F 5F 33 00 00 ) // ler+<Test>d__3..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3' V_0,
             class [mscorlib]System.Threading.Tasks.Task`1<string> V_1,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> V_2)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldarg.1
    IL_000b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::test
    IL_0010:  ldloca.s   V_0
    IL_0012:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Create()
    IL_0017:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldc.i4.m1
    IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>1__state'
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
    IL_002b:  stloc.2
    IL_002c:  ldloca.s   V_2
    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'>(!!0&)
    IL_0035:  ldloca.s   V_0
    IL_0037:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler/'<Test>d__3'::'<>t__builder'
    IL_003c:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::get_Task()
    IL_0041:  stloc.1
    IL_0042:  br.s       IL_0044

    IL_0044:  ldloc.1
    IL_0045:  ret
  } // end of method AwaitTestHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler::.ctor()
    IL_0006:  ret
  } // end of method AwaitTestHandler::.ctor

  .method private hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          '<>n__FabricatedMethod5'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (class [mscorlib]System.Threading.Tasks.Task`1<string> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitDelegatingHandler::Test(string)
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method AwaitTestHandler::'<>n__FabricatedMethod5'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.AwaitTestHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance string  Test(string test) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  4
    .locals init (string V_0,
             string[] V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  newarr     [mscorlib]System.String
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldc.i4.0
    IL_000b:  ldstr      "fsdf"
    IL_0010:  stelem.ref
    IL_0011:  ldloc.1
    IL_0012:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0017:  stloc.0
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.0
    IL_001b:  ret
  } // end of method YieldDelegatingHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method YieldDelegatingHandler::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler
{
  .class auto ansi sealed nested private beforefieldinit '<Test2>d__0'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<string>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler '<>4__this'
    .field public string test
    .field public string '<>3__test'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       82 (0x52)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  ceq
      IL_0017:  ldc.i4.0
      IL_0018:  ceq
      IL_001a:  br.s       IL_001d

      IL_001c:  ldc.i4.1
      IL_001d:  nop
      IL_001e:  stloc.2
      IL_001f:  ldloc.2
      IL_0020:  brtrue.s   IL_002d

      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.0
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0040

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
      IL_003b:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
      IL_0040:  ldloc.0
      IL_0041:  ldarg.0
      IL_0042:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>3__test'
      IL_0047:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::test
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  br.s       IL_0050

      IL_0050:  ldloc.1
      IL_0051:  ret
    } // end of method '<Test2>d__0'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class [mscorlib]System.Collections.IEnumerator V_0)
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<Test2>d__0'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       85 (0x55)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0019,
                            IL_0017)
      IL_0015:  br.s       IL_001b

      IL_0017:  br.s       IL_0047

      IL_0019:  br.s       IL_001d

      IL_001b:  br.s       IL_004f

      IL_001d:  ldarg.0
      IL_001e:  ldc.i4.m1
      IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldarg.0
      IL_0027:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
      IL_002c:  ldarg.0
      IL_002d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::test
      IL_0032:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler::'<>n__FabricatedMethod1'(string)
      IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>2__current'
      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_0043:  ldc.i4.1
      IL_0044:  stloc.0
      IL_0045:  br.s       IL_0053

      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.m1
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_004e:  nop
      IL_004f:  ldc.i4.0
      IL_0050:  stloc.0
      IL_0051:  br.s       IL_0053

      IL_0053:  ldloc.0
      IL_0054:  ret
    } // end of method '<Test2>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<Test2>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<Test2>d__0'::System.Collections.IEnumerator.get_Current

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 '<>1__state') cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       25 (0x19)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<Test2>d__0'::.ctor

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
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>4__this'
    IL_000f:  ldloc.0
    IL_0010:  ldarg.1
    IL_0011:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler/'<Test2>d__0'::'<>3__test'
    IL_0016:  ldloc.0
    IL_0017:  stloc.1
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.1
    IL_001b:  ret
  } // end of method YieldTestHandler::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler::.ctor()
    IL_0006:  ret
  } // end of method YieldTestHandler::.ctor

  .method private hidebysig instance string 
          '<>n__FabricatedMethod1'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldDelegatingHandler::Test(string)
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method YieldTestHandler::'<>n__FabricatedMethod1'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.YieldTestHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance string  Test(string test) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  4
    .locals init (string V_0,
             string[] V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  newarr     [mscorlib]System.String
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldc.i4.0
    IL_000b:  ldstr      "fsdf"
    IL_0010:  stelem.ref
    IL_0011:  ldloc.1
    IL_0012:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0017:  stloc.0
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.0
    IL_001b:  ret
  } // end of method LambdaDelegatingHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method LambdaDelegatingHandler::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler '<>4__this'
    .field public string test
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1'::.ctor

    .method public hidebysig instance string 
            '<Test>b__0'() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  2
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1'::'<>4__this'
      IL_0006:  ldarg.0
      IL_0007:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1'::test
      IL_000c:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler::'<>n__FabricatedMethod3'(string)
      IL_0011:  stloc.0
      IL_0012:  br.s       IL_0014

      IL_0014:  ldloc.0
      IL_0015:  ret
    } // end of method '<>c__DisplayClass1'::'<Test>b__0'

  } // end of class '<>c__DisplayClass1'

  .method famorassem hidebysig virtual instance string 
          Test(string test) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (class [mscorlib]System.Func`1<string> V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1' V_1,
             string V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1'::.ctor()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldarg.1
    IL_0008:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1'::test
    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1'::'<>4__this'
    IL_0014:  nop
    IL_0015:  ldloc.1
    IL_0016:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler/'<>c__DisplayClass1'::'<Test>b__0'()
    IL_001c:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  callvirt   instance !0 class [mscorlib]System.Func`1<string>::Invoke()
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.2
    IL_002c:  ret
  } // end of method LambdaHandler::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler::.ctor()
    IL_0006:  ret
  } // end of method LambdaHandler::.ctor

  .method private hidebysig instance string 
          '<>n__FabricatedMethod3'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaDelegatingHandler::Test(string)
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method LambdaHandler::'<>n__FabricatedMethod3'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LambdaHandler


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// Warnung: Win32-Ressourcendatei "../../../TestCases/Pretty\FixProxyCalls.res" wurde erstellt.
