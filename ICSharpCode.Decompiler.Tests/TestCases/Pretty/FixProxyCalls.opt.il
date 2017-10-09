
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly b0rqcnnq
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module b0rqcnnq.dll
// MVID: {D8989CD1-06F3-4E8C-8C4C-7F7B95ACB488}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00810000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A
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
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass1'::test
      IL_0006:  callvirt   instance string [mscorlib]System.String::ToUpper()
      IL_000b:  ret
    } // end of method '<>c__DisplayClass1'::'<Test>b__0'

  } // end of class '<>c__DisplayClass1'

  .method famorassem hidebysig newslot virtual 
          instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          Test(string test) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass1' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass1'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass1'::test
    IL_000d:  ldloc.0
    IL_000e:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A/'<>c__DisplayClass1'::'<Test>b__0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_0019:  call       class [mscorlib]System.Threading.Tasks.Task`1<!!0> [mscorlib]System.Threading.Tasks.Task::Run<string>(class [mscorlib]System.Func`1<!!0>)
    IL_001e:  ret
  } // end of method A::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method A::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__0'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B '<>4__this'
    .field public string test
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> '<>u__$awaiter1'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       172 (0xac)
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
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_0008:  stloc.3
        IL_0009:  ldloc.3
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_004f

        IL_000d:  ldarg.0
        IL_000e:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>4__this'
        IL_0013:  ldarg.0
        IL_0014:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::test
        IL_0019:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B::'<>n__FabricatedMethod2'(string)
        IL_001e:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<string>::GetAwaiter()
        IL_0023:  stloc.s    V_4
        IL_0025:  ldloca.s   V_4
        IL_0027:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::get_IsCompleted()
        IL_002c:  brtrue.s   IL_006e

        IL_002e:  ldarg.0
        IL_002f:  ldc.i4.0
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_0035:  ldarg.0
        IL_0036:  ldloc.s    V_4
        IL_0038:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>u__$awaiter1'
        IL_003d:  ldarg.0
        IL_003e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
        IL_0043:  ldloca.s   V_4
        IL_0045:  ldarg.0
        IL_0046:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>,valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                                            !!1&)
        IL_004b:  ldc.i4.0
        IL_004c:  stloc.0
        IL_004d:  leave.s    IL_00ab

        IL_004f:  ldarg.0
        IL_0050:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>u__$awaiter1'
        IL_0055:  stloc.s    V_4
        IL_0057:  ldarg.0
        IL_0058:  ldloca.s   V_5
        IL_005a:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>
        IL_0060:  ldloc.s    V_5
        IL_0062:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>u__$awaiter1'
        IL_0067:  ldarg.0
        IL_0068:  ldc.i4.m1
        IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_006e:  ldloca.s   V_4
        IL_0070:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>::GetResult()
        IL_0075:  ldloca.s   V_4
        IL_0077:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<string>
        IL_007d:  stloc.1
        IL_007e:  leave.s    IL_0097

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0080:  stloc.2
        IL_0081:  ldarg.0
        IL_0082:  ldc.i4.s   -2
        IL_0084:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
        IL_0089:  ldarg.0
        IL_008a:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
        IL_008f:  ldloc.2
        IL_0090:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetException(class [mscorlib]System.Exception)
        IL_0095:  leave.s    IL_00ab

      }  // end handler
      IL_0097:  ldarg.0
      IL_0098:  ldc.i4.s   -2
      IL_009a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
      IL_009f:  ldarg.0
      IL_00a0:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
      IL_00a5:  ldloc.1
      IL_00a6:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetResult(!0)
      IL_00ab:  ret
    } // end of method '<Test>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
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
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldarg.1
    IL_000b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::test
    IL_0010:  ldloca.s   V_0
    IL_0012:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Create()
    IL_0017:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldc.i4.m1
    IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>1__state'
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
    IL_002b:  stloc.1
    IL_002c:  ldloca.s   V_1
    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'>(!!0&)
    IL_0035:  ldloca.s   V_0
    IL_0037:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B/'<Test>d__0'::'<>t__builder'
    IL_003c:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>::get_Task()
    IL_0041:  ret
  } // end of method B::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A::.ctor()
    IL_0006:  ret
  } // end of method B::.ctor

  .method private hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<string> 
          '<>n__FabricatedMethod2'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<string> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.A::Test(string)
    IL_0007:  ret
  } // end of method B::'<>n__FabricatedMethod2'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.B

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance string  Test(string test) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init (string[] V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "fsdf"
    IL_000f:  stelem.ref
    IL_0010:  ldloc.0
    IL_0011:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0016:  ret
  } // end of method C::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
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
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D '<>4__this'
    .field public string test
    .field public string '<>3__test'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
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

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       71 (0x47)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_003e)
      IL_0015:  br.s       IL_0045

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.m1
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  ldarg.0
      IL_0020:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>4__this'
      IL_0025:  ldarg.0
      IL_0026:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::test
      IL_002b:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D::'<>n__FabricatedMethod1'(string)
      IL_0030:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>2__current'
      IL_0035:  ldarg.0
      IL_0036:  ldc.i4.1
      IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_003c:  ldc.i4.1
      IL_003d:  ret

      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.m1
      IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_0045:  ldc.i4.0
      IL_0046:  ret
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

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test2>d__0'::System.IDisposable.Dispose

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<Test2>d__0'::.ctor

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
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>4__this'
    IL_000f:  ldloc.0
    IL_0010:  ldarg.1
    IL_0011:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D/'<Test2>d__0'::'<>3__test'
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method D::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C::.ctor()
    IL_0006:  ret
  } // end of method D::.ctor

  .method private hidebysig instance string 
          '<>n__FabricatedMethod1'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.C::Test(string)
    IL_0007:  ret
  } // end of method D::'<>n__FabricatedMethod1'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.D

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance string  Test(string test) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init (string[] V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "fsdf"
    IL_000f:  stelem.ref
    IL_0010:  ldloc.0
    IL_0011:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0016:  ret
  } // end of method E::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method E::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E
{
  .method famorassem hidebysig virtual instance string 
          Test(string test) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  4
    .locals init (class [mscorlib]System.Func`2<string,string> V_0,
             string[] V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F::'<Test>b__0'(string)
    IL_0007:  newobj     instance void class [mscorlib]System.Func`2<string,string>::.ctor(object,
                                                                                           native int)
    IL_000c:  stloc.0
    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.1
    IL_000f:  newarr     [mscorlib]System.String
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.0
    IL_0017:  ldstr      "aa"
    IL_001c:  stelem.ref
    IL_001d:  ldloc.1
    IL_001e:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0023:  starg.s    test
    IL_0025:  ldloc.0
    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance !1 class [mscorlib]System.Func`2<string,string>::Invoke(!0)
    IL_002c:  ret
  } // end of method F::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E::.ctor()
    IL_0006:  ret
  } // end of method F::.ctor

  .method private hidebysig instance string 
          '<>n__FabricatedMethod1'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.E::Test(string)
    IL_0007:  ret
  } // end of method F::'<>n__FabricatedMethod1'

  .method private hidebysig instance string 
          '<Test>b__0'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F::'<>n__FabricatedMethod1'(string)
    IL_0007:  ret
  } // end of method F::'<Test>b__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.F

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance void  Test(string test) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  4
    .locals init (string[] V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "fsdf"
    IL_000f:  stelem.ref
    IL_0010:  ldloc.0
    IL_0011:  call       string [mscorlib]System.String::Join(string,
                                                              string[])
    IL_0016:  pop
    IL_0017:  ret
  } // end of method G::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method G::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G
{
  .method famorassem hidebysig virtual instance void 
          Test(string test) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (class [mscorlib]System.Action`1<string> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H::'<Test>b__0'(string)
    IL_0007:  newobj     instance void class [mscorlib]System.Action`1<string>::.ctor(object,
                                                                                      native int)
    IL_000c:  stloc.0
    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.1
    IL_000f:  box        [mscorlib]System.Int32
    IL_0014:  callvirt   instance bool [mscorlib]System.Object::Equals(object)
    IL_0019:  brfalse.s  IL_0026

    IL_001b:  ldstr      "roslyn optimize is inlining the assignment which l"
    + "ets the test fail"
    IL_0020:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0025:  throw

    IL_0026:  ldloc.0
    IL_0027:  ldarg.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Action`1<string>::Invoke(!0)
    IL_002d:  ret
  } // end of method H::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G::.ctor()
    IL_0006:  ret
  } // end of method H::.ctor

  .method private hidebysig instance void 
          '<>n__FabricatedMethod1'(string A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.G::Test(string)
    IL_0007:  ret
  } // end of method H::'<>n__FabricatedMethod1'

  .method private hidebysig instance void 
          '<Test>b__0'(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H::'<>n__FabricatedMethod1'(string)
    IL_0007:  ret
  } // end of method H::'<Test>b__0'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.H

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I
       extends [mscorlib]System.Object
{
  .method famorassem hidebysig newslot virtual 
          instance void  Test(int32 a) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method I::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method I::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J
       extends ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I
{
  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J '<>4__this'
    .field public int32 a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1'::.ctor

    .method public hidebysig instance void 
            '<Test>b__0'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::'<>4__this'
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::a
      IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J::'<>n__FabricatedMethod3'(int32)
      IL_0011:  ret
    } // end of method '<>c__DisplayClass1'::'<Test>b__0'

  } // end of class '<>c__DisplayClass1'

  .method famorassem hidebysig virtual instance void 
          Test(int32 a) cil managed
  {
    // Code size       65 (0x41)
    .maxstack  2
    .locals init (class [mscorlib]System.Action V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1' V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::.ctor()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::a
    IL_000d:  ldloc.1
    IL_000e:  ldarg.0
    IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::'<>4__this'
    IL_0014:  ldloc.1
    IL_0015:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::'<Test>b__0'()
    IL_001b:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0020:  stloc.0
    IL_0021:  ldloc.1
    IL_0022:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J/'<>c__DisplayClass1'::a
    IL_0027:  ldc.i4.1
    IL_0028:  call       instance bool [mscorlib]System.Int32::Equals(int32)
    IL_002d:  brfalse.s  IL_003a

    IL_002f:  ldstr      "roslyn optimize is inlining the assignment which l"
    + "ets the test fail"
    IL_0034:  newobj     instance void [mscorlib]System.Exception::.ctor(string)
    IL_0039:  throw

    IL_003a:  ldloc.0
    IL_003b:  callvirt   instance void [mscorlib]System.Action::Invoke()
    IL_0040:  ret
  } // end of method J::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I::.ctor()
    IL_0006:  ret
  } // end of method J::.ctor

  .method private hidebysig instance void 
          '<>n__FabricatedMethod3'(int32 A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.I::Test(int32)
    IL_0007:  ret
  } // end of method J::'<>n__FabricatedMethod3'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.J

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<Test>d__0'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public int32 p
    .field public int32 '<>3__p'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K '<>4__this'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
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

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       96 (0x60)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_0039,
                            IL_0057)
      IL_0019:  br.s       IL_005e

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.m1
      IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0022:  ldarg.0
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::p
      IL_0029:  ldc.i4.1
      IL_002a:  add
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>2__current'
      IL_0030:  ldarg.0
      IL_0031:  ldc.i4.1
      IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0037:  ldc.i4.1
      IL_0038:  ret

      IL_0039:  ldarg.0
      IL_003a:  ldc.i4.m1
      IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0040:  ldarg.0
      IL_0041:  ldarg.0
      IL_0042:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::p
      IL_0047:  ldc.i4.2
      IL_0048:  add
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>2__current'
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.2
      IL_0050:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_0055:  ldc.i4.1
      IL_0056:  ret

      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.m1
      IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_005e:  ldc.i4.0
      IL_005f:  ret
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

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test>d__0'::System.IDisposable.Dispose

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<Test>d__0'::.ctor

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
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>4__this'
    IL_000f:  ldloc.0
    IL_0010:  ldarg.1
    IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K/'<Test>d__0'::'<>3__p'
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method K::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
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
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L '<>4__this'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
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
      IL_0036:  ret
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

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       97 (0x61)
      .maxstack  4
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_0058)
      IL_0015:  br.s       IL_005f

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.m1
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  ldarg.0
      IL_0020:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
      IL_0025:  ldarg.0
      IL_0026:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
      IL_002b:  ldc.i4.0
      IL_002c:  call       instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L::'<>n__FabricatedMethod1'(int32)
      IL_0031:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0036:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_003b:  call       instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L::'<>n__FabricatedMethod1'(int32)
      IL_0040:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0045:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_004a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>2__current'
      IL_004f:  ldarg.0
      IL_0050:  ldc.i4.1
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_0056:  ldc.i4.1
      IL_0057:  ret

      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.m1
      IL_005a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_005f:  ldc.i4.0
      IL_0060:  ret
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

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Test>d__0'::System.IDisposable.Dispose

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<Test>d__0'::.ctor

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
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L/'<Test>d__0'::'<>4__this'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method L::Test

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K::.ctor()
    IL_0006:  ret
  } // end of method L::.ctor

  .method private hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          '<>n__FabricatedMethod1'(int32 A_1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.K::Test(int32)
    IL_0007:  ret
  } // end of method L::'<>n__FabricatedMethod1'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.L


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// Warnung: Win32-Ressourcendatei "../../../TestCases/Pretty\FixProxyCalls.opt.res" wurde erstellt.
