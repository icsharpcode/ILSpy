
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
// MVID: {337B20CD-03F0-410E-9868-F4B4F95C9AEF}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00DC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A
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
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass0_0'::test
      IL_0006:  callvirt   instance string [mscorlib]System.String::ToUpper()
      IL_000b:  ret
    } // end of method '<>c__DisplayClass0_0'::'<Test>b__0'

  } // end of class '<>c__DisplayClass0_0'

  .method famorassem hidebysig newslot virtual 
          instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          Test(string test) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass0_0' V_0,
             class [mscorlib]System.Threading.Tasks.Task`1<string> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass0_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass0_0'::test
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass0_0'::'<Test>b__0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_001a:  call       class [mscorlib]System.Threading.Tasks.Task`1<!!0> [mscorlib]System.Threading.Tasks.Task::Run<string>(class [mscorlib]System.Func`1<!!0>)
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method A::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method A::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__0'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> '<>t__builder'
    .field public string test
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B '<>4__this'
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
               class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0' V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_0053

        IL_000e:  nop
        IL_000f:  ldarg.0
        IL_0010:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>4__this'
        IL_0015:  ldarg.0
        IL_0016:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::test
        IL_001b:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B::'<>n__0'(string)
        IL_0020:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<string>::GetAwaiter()
        IL_0025:  stloc.2
        IL_0026:  ldloca.s   V_2
        IL_0028:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::get_IsCompleted()
        IL_002d:  brtrue.s   IL_006f

        IL_002f:  ldarg.0
        IL_0030:  ldc.i4.0
        IL_0031:  dup
        IL_0032:  stloc.0
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_0038:  ldarg.0
        IL_0039:  ldloc.2
        IL_003a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>u__1'
        IL_003f:  ldarg.0
        IL_0040:  stloc.3
        IL_0041:  ldarg.0
        IL_0042:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
        IL_0047:  ldloca.s   V_2
        IL_0049:  ldloca.s   V_3
        IL_004b:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>,class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                                        !!1&)
        IL_0050:  nop
        IL_0051:  leave.s    IL_00b4

        IL_0053:  ldarg.0
        IL_0054:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>u__1'
        IL_0059:  stloc.2
        IL_005a:  ldarg.0
        IL_005b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>u__1'
        IL_0060:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>
        IL_0066:  ldarg.0
        IL_0067:  ldc.i4.m1
        IL_0068:  dup
        IL_0069:  stloc.0
        IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_006f:  ldarg.0
        IL_0070:  ldloca.s   V_2
        IL_0072:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::GetResult()
        IL_0077:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>s__1'
        IL_007c:  ldarg.0
        IL_007d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>s__1'
        IL_0082:  stloc.1
        IL_0083:  leave.s    IL_009f

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0085:  stloc.s    V_4
        IL_0087:  ldarg.0
        IL_0088:  ldc.i4.s   -2
        IL_008a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_008f:  ldarg.0
        IL_0090:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
        IL_0095:  ldloc.s    V_4
        IL_0097:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetException(class [mscorlib]System.Exception)
        IL_009c:  nop
        IL_009d:  leave.s    IL_00b4

      }  // end handler
      IL_009f:  ldarg.0
      IL_00a0:  ldc.i4.s   -2
      IL_00a2:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
      IL_00a7:  ldarg.0
      IL_00a8:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
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
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 3C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..<ICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                       74 79 2E 42 2B 3C 54 65 73 74 3E 64 5F 5F 30 00   // ty.B+<Test>d__0.
                                                                                                                                       00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       66 (0x42)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::test
    IL_0014:  ldloc.0
    IL_0015:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Create()
    IL_001a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.m1
    IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
    IL_0026:  ldloc.0
    IL_0027:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloca.s   V_0
    IL_0031:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Start<class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'>(!!0&)
    IL_0036:  ldloc.0
    IL_0037:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
    IL_003c:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::get_Task()
    IL_0041:  ret
  } // end of method B::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method B::.ctor

  .method private hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          '<>n__0'(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A::Test(string)
    IL_0007:  ret
  } // end of method B::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C
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
  } // end of method C::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method C::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C
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
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D '<>4__this'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>l__initialThreadId'
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
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
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
      IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_001f:  nop
      IL_0020:  ldarg.0
      IL_0021:  ldarg.0
      IL_0022:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>4__this'
      IL_0027:  ldarg.0
      IL_0028:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::test
      IL_002d:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D::'<>n__0'(string)
      IL_0032:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.m1
      IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
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
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>2__current'
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
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>2__current'
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
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>4__this'
      IL_0030:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>3__test'
      IL_003c:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::test
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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<Test2>d__0'::System.Collections.IEnumerable.GetEnumerator

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<Test2>d__0'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::System.Collections.IEnumerator.get_Current()
    } // end of property '<Test2>d__0'::System.Collections.IEnumerator.Current
  } // end of class '<Test2>d__0'

  .method famorassem hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          Test2(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 3D 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..=ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                          74 79 2E 44 2B 3C 54 65 73 74 32 3E 64 5F 5F 30   // ty.D+<Test2>d__0
                                                                                                                                          00 00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>4__this'
    IL_000e:  dup
    IL_000f:  ldarg.1
    IL_0010:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>3__test'
    IL_0015:  ret
  } // end of method D::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method D::.ctor

  .method private hidebysig instance string 
          '<>n__0'(string test) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C::Test(string)
    IL_0007:  ret
  } // end of method D::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E
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
  } // end of method E::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method E::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E
{
  .method famorassem hidebysig virtual instance string 
          Test(string test) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  5
    .locals init (class [mscorlib]System.Func`2<string,string> V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F::'<Test>b__0_0'(string)
    IL_0008:  newobj     instance void class [mscorlib]System.Func`2<string,string>::.ctor(object,
                                                                                           native int)
    IL_000d:  stloc.0
    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.1
    IL_0010:  newarr     [mscorlib]System.String
    IL_0015:  dup
    IL_0016:  ldc.i4.0
    IL_0017:  ldstr      "aa"
    IL_001c:  stelem.ref
    IL_001d:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0022:  starg.s    test
    IL_0024:  ldloc.0
    IL_0025:  ldarg.1
    IL_0026:  callvirt   instance !1 class [mscorlib]System.Func`2<string,string>::Invoke(!0)
    IL_002b:  stloc.1
    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.1
    IL_002f:  ret
  } // end of method F::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method F::.ctor

  .method private hidebysig instance string 
          '<Test>b__0_0'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E::Test(string)
    IL_0007:  ret
  } // end of method F::'<Test>b__0_0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance void  Test(string test) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  8
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
    IL_0015:  pop
    IL_0016:  ret
  } // end of method G::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method G::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G
{
  .method famorassem hidebysig virtual instance void 
          Test(string test) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (class [mscorlib]System.Action`1<string> V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H::'<Test>b__0_0'(string)
    IL_0008:  newobj     instance void class [mscorlib]System.Action`1<string>::.ctor(object,
                                                                                      native int)
    IL_000d:  stloc.0
    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.1
    IL_0010:  box        [mscorlib]System.Int32
    IL_0015:  callvirt   instance bool [mscorlib]System.Object::Equals(object)
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brfalse.s  IL_002a

    IL_001e:  nop
    IL_001f:  ldstr      "roslyn optimize is inlining the assignment which l"
    + "ets the test fail"
    IL_0024:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0029:  throw

    IL_002a:  ldloc.0
    IL_002b:  ldarg.1
    IL_002c:  callvirt   instance void class [mscorlib]System.Action`1<string>::Invoke(!0)
    IL_0031:  nop
    IL_0032:  ret
  } // end of method H::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method H::.ctor

  .method private hidebysig instance void 
          '<Test>b__0_0'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G::Test(string)
    IL_0008:  nop
    IL_0009:  ret
  } // end of method H::'<Test>b__0_0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance void  Test(int32 a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method I::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method I::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass0_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 a
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J '<>4__this'
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

    .method assembly hidebysig instance void 
            '<Test>b__0'() cil managed
    {
      // Code size       20 (0x14)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::'<>4__this'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::a
      IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J::'<>n__0'(int32)
      IL_0012:  nop
      IL_0013:  ret
    } // end of method '<>c__DisplayClass0_0'::'<Test>b__0'

  } // end of class '<>c__DisplayClass0_0'

  .method famorassem hidebysig virtual instance void 
          Test(int32 a) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0' V_0,
             class [mscorlib]System.Action V_1,
             bool V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::a
    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::'<Test>b__0'()
    IL_001c:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0021:  stloc.1
    IL_0022:  ldloc.0
    IL_0023:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass0_0'::a
    IL_0028:  ldc.i4.1
    IL_0029:  call       instance bool [mscorlib]System.Int32::Equals(int32)
    IL_002e:  stloc.2
    IL_002f:  ldloc.2
    IL_0030:  brfalse.s  IL_003e

    IL_0032:  nop
    IL_0033:  ldstr      "roslyn optimize is inlining the assignment which l"
    + "ets the test fail"
    IL_0038:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_003d:  throw

    IL_003e:  ldloc.1
    IL_003f:  callvirt   instance void [mscorlib]System.Action::Invoke()
    IL_0044:  nop
    IL_0045:  ret
  } // end of method J::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method J::.ctor

  .method private hidebysig instance void 
          '<>n__0'(int32 a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I::Test(int32)
    IL_0007:  ret
  } // end of method J::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__0'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private int32 '<>2__current'
    .field private int32 '<>l__initialThreadId'
    .field private int32 p
    .field public int32 '<>3__p'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K '<>4__this'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<Test>d__0'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       105 (0x69)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_0042

      IL_001f:  br.s       IL_0060

      IL_0021:  ldc.i4.0
      IL_0022:  ret

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::p
      IL_0032:  ldc.i4.1
      IL_0033:  add
      IL_0034:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>2__current'
      IL_0039:  ldarg.0
      IL_003a:  ldc.i4.1
      IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0040:  ldc.i4.1
      IL_0041:  ret

      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.m1
      IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0049:  ldarg.0
      IL_004a:  ldarg.0
      IL_004b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::p
      IL_0050:  ldc.i4.2
      IL_0051:  add
      IL_0052:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>2__current'
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_005e:  ldc.i4.1
      IL_005f:  ret

      IL_0060:  ldarg.0
      IL_0061:  ldc.i4.m1
      IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0067:  ldc.i4.0
      IL_0068:  ret
    } // end of method '<Test>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>2__current'
      IL_0006:  ret
    } // end of method '<Test>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<Test>d__0'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<Test>d__0'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>4__this'
      IL_0030:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>3__p'
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::p
      IL_0041:  ldloc.0
      IL_0042:  ret
    } // end of method '<Test>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<Test>d__0'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<Test>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::System.Collections.IEnumerator.get_Current()
    } // end of property '<Test>d__0'::System.Collections.IEnumerator.Current
  } // end of class '<Test>d__0'

  .method famorassem hidebysig newslot virtual 
          instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          Test(int32 p) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 3C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..<ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                          74 79 2E 4B 2B 3C 54 65 73 74 3E 64 5F 5F 30 00   // ty.K+<Test>d__0.
                                                                                                                                          00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>4__this'
    IL_000e:  dup
    IL_000f:  ldarg.1
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>3__p'
    IL_0015:  ret
  } // end of method K::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method K::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__0'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private int32 '<>2__current'
    .field private int32 '<>l__initialThreadId'
    .field private int32 p
    .field public int32 '<>3__p'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L '<>4__this'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<Test>d__0'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       99 (0x63)
      .maxstack  4
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  br.s       IL_000c

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  beq.s      IL_0014

      IL_0010:  br.s       IL_0016

      IL_0012:  br.s       IL_0018

      IL_0014:  br.s       IL_005a

      IL_0016:  ldc.i4.0
      IL_0017:  ret

      IL_0018:  ldarg.0
      IL_0019:  ldc.i4.m1
      IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_001f:  nop
      IL_0020:  ldarg.0
      IL_0021:  ldarg.0
      IL_0022:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
      IL_0027:  ldarg.0
      IL_0028:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
      IL_002d:  ldc.i4.0
      IL_002e:  call       instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L::'<>n__0'(int32)
      IL_0033:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0038:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_003d:  call       instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L::'<>n__0'(int32)
      IL_0042:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0047:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>2__current'
      IL_0051:  ldarg.0
      IL_0052:  ldc.i4.1
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0058:  ldc.i4.1
      IL_0059:  ret

      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.m1
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0061:  ldc.i4.0
      IL_0062:  ret
    } // end of method '<Test>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>2__current'
      IL_0006:  ret
    } // end of method '<Test>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<Test>d__0'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<Test>d__0'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
      IL_0030:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>3__p'
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::p
      IL_0041:  ldloc.0
      IL_0042:  ret
    } // end of method '<Test>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<Test>d__0'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<Test>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::System.Collections.IEnumerator.get_Current()
    } // end of property '<Test>d__0'::System.Collections.IEnumerator.Current
  } // end of class '<Test>d__0'

  .method famorassem hidebysig virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          Test(int32 p) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 3C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..<ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 49 4C 50 72 65 74   // TestCases.ILPret
                                                                                                                                          74 79 2E 4C 2B 3C 54 65 73 74 3E 64 5F 5F 30 00   // ty.L+<Test>d__0.
                                                                                                                                          00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
    IL_000e:  dup
    IL_000f:  ldarg.1
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>3__p'
    IL_0015:  ret
  } // end of method L::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method L::.ctor

  .method private hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          '<>n__0'(int32 p) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K::Test(int32)
    IL_0007:  ret
  } // end of method L::'<>n__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
