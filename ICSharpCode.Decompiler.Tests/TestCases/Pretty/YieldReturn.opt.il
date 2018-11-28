



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly YieldReturn.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module YieldReturn.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<get_YieldChars>d__0'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<char>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<char>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private char '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> 
            'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<char>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<get_YieldChars>d__0'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       112 (0x70)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_0037,
                            IL_004f,
                            IL_0067)
      IL_001d:  br.s       IL_006e

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldc.i4.s   97
      IL_0029:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.1
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.m1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.s   98
      IL_0041:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0046:  ldarg.0
      IL_0047:  ldc.i4.2
      IL_0048:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_004d:  ldc.i4.1
      IL_004e:  ret

      IL_004f:  ldarg.0
      IL_0050:  ldc.i4.m1
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0056:  ldarg.0
      IL_0057:  ldc.i4.s   99
      IL_0059:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_005e:  ldarg.0
      IL_005f:  ldc.i4.3
      IL_0060:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0065:  ldc.i4.1
      IL_0066:  ret

      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.m1
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_006e:  ldc.i4.0
      IL_006f:  ret
    } // end of method '<get_YieldChars>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance char  'System.Collections.Generic.IEnumerator<System.Char>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<char>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0006:  ret
    } // end of method '<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<get_YieldChars>d__0'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<get_YieldChars>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Char
      IL_000b:  ret
    } // end of method '<get_YieldChars>d__0'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<get_YieldChars>d__0'::.ctor

    .property instance char 'System.Collections.Generic.IEnumerator<System.Char>.Current'()
    {
      .get instance char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'()
    } // end of property '<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerator<System.Char>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::System.Collections.IEnumerator.get_Current()
    } // end of property '<get_YieldChars>d__0'::System.Collections.IEnumerator.Current
  } // end of class '<get_YieldChars>d__0'

  .class auto ansi sealed nested private beforefieldinit '<SimpleYieldReturn>d__3'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<SimpleYieldReturn>d__3'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       121 (0x79)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_003a,
                            IL_0055,
                            IL_0070)
      IL_001d:  br.s       IL_0077

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldstr      "A"
      IL_002c:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0031:  ldarg.0
      IL_0032:  ldc.i4.1
      IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0038:  ldc.i4.1
      IL_0039:  ret

      IL_003a:  ldarg.0
      IL_003b:  ldc.i4.m1
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0041:  ldarg.0
      IL_0042:  ldstr      "B"
      IL_0047:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.2
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0053:  ldc.i4.1
      IL_0054:  ret

      IL_0055:  ldarg.0
      IL_0056:  ldc.i4.m1
      IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_005c:  ldarg.0
      IL_005d:  ldstr      "C"
      IL_0062:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.3
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_006e:  ldc.i4.1
      IL_006f:  ret

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.m1
      IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0077:  ldc.i4.0
      IL_0078:  ret
    } // end of method '<SimpleYieldReturn>d__3'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<SimpleYieldReturn>d__3'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleYieldReturn>d__3'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturn>d__3'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<SimpleYieldReturn>d__3'::.ctor

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::System.Collections.IEnumerator.get_Current()
    } // end of property '<SimpleYieldReturn>d__3'::System.Collections.IEnumerator.Current
  } // end of class '<SimpleYieldReturn>d__3'

  .class auto ansi sealed nested private beforefieldinit '<SimpleYieldReturnEnumerator>d__6'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<>2__current'
    .field private int32 '<>1__state'
    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       121 (0x79)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_003a,
                            IL_0055,
                            IL_0070)
      IL_001d:  br.s       IL_0077

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldstr      "A"
      IL_002c:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0031:  ldarg.0
      IL_0032:  ldc.i4.1
      IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0038:  ldc.i4.1
      IL_0039:  ret

      IL_003a:  ldarg.0
      IL_003b:  ldc.i4.m1
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0041:  ldarg.0
      IL_0042:  ldstr      "B"
      IL_0047:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.2
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0053:  ldc.i4.1
      IL_0054:  ret

      IL_0055:  ldarg.0
      IL_0056:  ldc.i4.m1
      IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_005c:  ldarg.0
      IL_005d:  ldstr      "C"
      IL_0062:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.3
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_006e:  ldc.i4.1
      IL_006f:  ret

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.m1
      IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0077:  ldc.i4.0
      IL_0078:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::System.Collections.IEnumerator.get_Current

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 '<>1__state') cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_000d:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::.ctor

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<SimpleYieldReturnEnumerator>d__6'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::System.Collections.IEnumerator.get_Current()
    } // end of property '<SimpleYieldReturnEnumerator>d__6'::System.Collections.IEnumerator.Current
  } // end of class '<SimpleYieldReturnEnumerator>d__6'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnParameters>d__8'
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
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest '<>4__this'
    .field public int32 p
    .field public int32 '<>3__p'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
      IL_0030:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>3__p'
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::p
      IL_0041:  ldloc.0
      IL_0042:  ret
    } // end of method '<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnParameters>d__8'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       97 (0x61)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_0037,
                            IL_0058)
      IL_0019:  br.s       IL_005f

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.m1
      IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0022:  ldarg.0
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::p
      IL_0029:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.1
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.m1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_003e:  ldarg.0
      IL_003f:  ldarg.0
      IL_0040:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
      IL_0045:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_004a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_004f:  ldarg.0
      IL_0050:  ldc.i4.2
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0056:  ldc.i4.1
      IL_0057:  ret

      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.m1
      IL_005a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_005f:  ldc.i4.0
      IL_0060:  ret
    } // end of method '<YieldReturnParameters>d__8'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnParameters>d__8'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldReturnParameters>d__8'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnParameters>d__8'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnParameters>d__8'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnParameters>d__8'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnParameters>d__8'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnParametersEnumerator>d__b'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>2__current'
    .field private int32 '<>1__state'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest '<>4__this'
    .field public int32 p
    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       97 (0x61)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_0037,
                            IL_0058)
      IL_0019:  br.s       IL_005f

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.m1
      IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0022:  ldarg.0
      IL_0023:  ldarg.0
      IL_0024:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::p
      IL_0029:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.1
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.m1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_003e:  ldarg.0
      IL_003f:  ldarg.0
      IL_0040:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>4__this'
      IL_0045:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_004a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_004f:  ldarg.0
      IL_0050:  ldc.i4.2
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0056:  ldc.i4.1
      IL_0057:  ret

      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.m1
      IL_005a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_005f:  ldc.i4.0
      IL_0060:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnParametersEnumerator>d__b'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::System.Collections.IEnumerator.get_Current

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 '<>1__state') cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_000d:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnParametersEnumerator>d__b'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnParametersEnumerator>d__b'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnParametersEnumerator>d__b'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnInLoop>d__d'
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
    .field public int32 '<i>5__e'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnInLoop>d__d'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       93 (0x5d)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_003c)
      IL_0015:  br.s       IL_005b

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.m1
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  ldc.i4.0
      IL_0020:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_0025:  br.s       IL_0051

      IL_0027:  ldarg.0
      IL_0028:  ldarg.0
      IL_0029:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_002e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>2__current'
      IL_0033:  ldarg.0
      IL_0034:  ldc.i4.1
      IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_003a:  ldc.i4.1
      IL_003b:  ret

      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.m1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0043:  ldarg.0
      IL_0044:  dup
      IL_0045:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_004a:  ldc.i4.1
      IL_004b:  add
      IL_004c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_0051:  ldarg.0
      IL_0052:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_0057:  ldc.i4.s   100
      IL_0059:  blt.s      IL_0027

      IL_005b:  ldc.i4.0
      IL_005c:  ret
    } // end of method '<YieldReturnInLoop>d__d'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnInLoop>d__d'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldReturnInLoop>d__d'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnInLoop>d__d'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnInLoop>d__d'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnInLoop>d__d'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnInLoop>d__d'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithTryFinally>d__11'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       143 (0x8f)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_003c,
                              IL_0082,
                              IL_005c,
                              IL_007b)
        IL_0021:  br.s       IL_0082

        IL_0023:  ldarg.0
        IL_0024:  ldc.i4.m1
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_002a:  ldarg.0
        IL_002b:  ldc.i4.0
        IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.1
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0038:  ldc.i4.1
        IL_0039:  stloc.0
        IL_003a:  leave.s    IL_008d

        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.m1
        IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.2
        IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_004a:  ldarg.0
        IL_004b:  ldc.i4.1
        IL_004c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
        IL_0051:  ldarg.0
        IL_0052:  ldc.i4.3
        IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0058:  ldc.i4.1
        IL_0059:  stloc.0
        IL_005a:  leave.s    IL_008d

        IL_005c:  ldarg.0
        IL_005d:  ldc.i4.2
        IL_005e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0063:  ldarg.0
        IL_0064:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>m__Finally12'()
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.2
        IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
        IL_0070:  ldarg.0
        IL_0071:  ldc.i4.4
        IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0077:  ldc.i4.1
        IL_0078:  stloc.0
        IL_0079:  leave.s    IL_008d

        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.m1
        IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0082:  ldc.i4.0
        IL_0083:  stloc.0
        IL_0084:  leave.s    IL_008d

      }  // end .try
      fault
      {
        IL_0086:  ldarg.0
        IL_0087:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::System.IDisposable.Dispose()
        IL_008c:  endfinally
      }  // end handler
      IL_008d:  ldloc.0
      IL_008e:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithTryFinally>d__11'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       34 (0x22)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.2
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0018,
                            IL_0018)
      IL_0017:  ret

      .try
      {
        IL_0018:  leave.s    IL_0021

      }  // end .try
      finally
      {
        IL_001a:  ldarg.0
        IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>m__Finally12'()
        IL_0020:  endfinally
      }  // end handler
      IL_0021:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally12'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_0007:  ldstr      "Finally!"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::'<>m__Finally12'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithTryFinally>d__11'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithTryFinally>d__11'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithNestedTryFinally>d__15'
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
    .field public bool breakInMiddle
    .field public bool '<>3__breakInMiddle'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>3__breakInMiddle'
      IL_0030:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::breakInMiddle
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       435 (0x1b3)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0036,
                              IL_0060,
                              IL_01a6,
                              IL_009b,
                              IL_01a6,
                              IL_00d6,
                              IL_0127,
                              IL_015e,
                              IL_0195)
        IL_0031:  br         IL_01a6

        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.m1
        IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_003d:  ldstr      "Start of method - 1"
        IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0047:  ldarg.0
        IL_0048:  ldstr      "Start of method"
        IL_004d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_0052:  ldarg.0
        IL_0053:  ldc.i4.1
        IL_0054:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0059:  ldc.i4.1
        IL_005a:  stloc.0
        IL_005b:  leave      IL_01b1

        IL_0060:  ldarg.0
        IL_0061:  ldc.i4.m1
        IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0067:  ldstr      "Start of method - 2"
        IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0071:  ldarg.0
        IL_0072:  ldc.i4.2
        IL_0073:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0078:  ldstr      "Within outer try - 1"
        IL_007d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0082:  ldarg.0
        IL_0083:  ldstr      "Within outer try"
        IL_0088:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_008d:  ldarg.0
        IL_008e:  ldc.i4.3
        IL_008f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0094:  ldc.i4.1
        IL_0095:  stloc.0
        IL_0096:  leave      IL_01b1

        IL_009b:  ldarg.0
        IL_009c:  ldc.i4.2
        IL_009d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00a2:  ldstr      "Within outer try - 2"
        IL_00a7:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00ac:  ldarg.0
        IL_00ad:  ldc.i4.4
        IL_00ae:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00b3:  ldstr      "Within inner try - 1"
        IL_00b8:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00bd:  ldarg.0
        IL_00be:  ldstr      "Within inner try"
        IL_00c3:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_00c8:  ldarg.0
        IL_00c9:  ldc.i4.5
        IL_00ca:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00cf:  ldc.i4.1
        IL_00d0:  stloc.0
        IL_00d1:  leave      IL_01b1

        IL_00d6:  ldarg.0
        IL_00d7:  ldc.i4.4
        IL_00d8:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00dd:  ldstr      "Within inner try - 2"
        IL_00e2:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00e7:  ldarg.0
        IL_00e8:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::breakInMiddle
        IL_00ed:  brfalse.s  IL_0104

        IL_00ef:  ldstr      "Breaking..."
        IL_00f4:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00f9:  ldarg.0
        IL_00fa:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::System.IDisposable.Dispose()
        IL_00ff:  leave      IL_01a6

        IL_0104:  ldstr      "End of inner try - 1"
        IL_0109:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_010e:  ldarg.0
        IL_010f:  ldstr      "End of inner try"
        IL_0114:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_0119:  ldarg.0
        IL_011a:  ldc.i4.6
        IL_011b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0120:  ldc.i4.1
        IL_0121:  stloc.0
        IL_0122:  leave      IL_01b1

        IL_0127:  ldarg.0
        IL_0128:  ldc.i4.4
        IL_0129:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_012e:  ldstr      "End of inner try - 2"
        IL_0133:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0138:  ldarg.0
        IL_0139:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally17'()
        IL_013e:  ldstr      "End of outer try - 1"
        IL_0143:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0148:  ldarg.0
        IL_0149:  ldstr      "End of outer try"
        IL_014e:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_0153:  ldarg.0
        IL_0154:  ldc.i4.7
        IL_0155:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_015a:  ldc.i4.1
        IL_015b:  stloc.0
        IL_015c:  leave.s    IL_01b1

        IL_015e:  ldarg.0
        IL_015f:  ldc.i4.2
        IL_0160:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0165:  ldstr      "End of outer try - 2"
        IL_016a:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_016f:  ldarg.0
        IL_0170:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally16'()
        IL_0175:  ldstr      "End of method - 1"
        IL_017a:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_017f:  ldarg.0
        IL_0180:  ldstr      "End of method"
        IL_0185:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_018a:  ldarg.0
        IL_018b:  ldc.i4.8
        IL_018c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0191:  ldc.i4.1
        IL_0192:  stloc.0
        IL_0193:  leave.s    IL_01b1

        IL_0195:  ldarg.0
        IL_0196:  ldc.i4.m1
        IL_0197:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_019c:  ldstr      "End of method - 2"
        IL_01a1:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01a6:  ldc.i4.0
        IL_01a7:  stloc.0
        IL_01a8:  leave.s    IL_01b1

      }  // end .try
      fault
      {
        IL_01aa:  ldarg.0
        IL_01ab:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::System.IDisposable.Dispose()
        IL_01b0:  endfinally
      }  // end handler
      IL_01b1:  ldloc.0
      IL_01b2:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       88 (0x58)
      .maxstack  2
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.2
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0028,
                            IL_0028,
                            IL_0028,
                            IL_0028,
                            IL_0028,
                            IL_0028)
      IL_0027:  ret

      .try
      {
        IL_0028:  ldarg.0
        IL_0029:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_002e:  stloc.1
        IL_002f:  ldloc.1
        IL_0030:  ldc.i4.4
        IL_0031:  sub
        IL_0032:  switch     ( 
                              IL_0045,
                              IL_0045,
                              IL_0045)
        IL_0043:  br.s       IL_004e

        .try
        {
          IL_0045:  leave.s    IL_004e

        }  // end .try
        finally
        {
          IL_0047:  ldarg.0
          IL_0048:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally17'()
          IL_004d:  endfinally
        }  // end handler
        IL_004e:  leave.s    IL_0057

      }  // end .try
      finally
      {
        IL_0050:  ldarg.0
        IL_0051:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally16'()
        IL_0056:  endfinally
      }  // end handler
      IL_0057:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally16'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0007:  ldstr      "Outer Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally16'

    .method private hidebysig instance void 
            '<>m__Finally17'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.2
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0007:  ldstr      "Inner Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally17'

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithNestedTryFinally>d__15'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithNestedTryFinally>d__15'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'
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
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<>3__input'
    .field public string '<line>5__1b'
    .field public string '<item>5__1c'
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap1d'
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap20'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>3__input'
      IL_0030:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::input
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       484 (0x1e4)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0042,
                              IL_01d7,
                              IL_01d7,
                              IL_0095,
                              IL_00ce,
                              IL_00ee,
                              IL_010e,
                              IL_012e,
                              IL_014e,
                              IL_016c,
                              IL_01d7,
                              IL_01bc)
        IL_003d:  br         IL_01d7

        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.m1
        IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0049:  ldarg.0
        IL_004a:  ldarg.0
        IL_004b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::input
        IL_0050:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0055:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
        IL_005a:  ldarg.0
        IL_005b:  ldc.i4.1
        IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0061:  br.s       IL_00a2

        IL_0063:  ldarg.0
        IL_0064:  ldarg.0
        IL_0065:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
        IL_006a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_006f:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<line>5__1b'
        IL_0074:  ldarg.0
        IL_0075:  ldc.i4.2
        IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_007b:  ldarg.0
        IL_007c:  ldarg.0
        IL_007d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<line>5__1b'
        IL_0082:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0087:  ldarg.0
        IL_0088:  ldc.i4.3
        IL_0089:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_008e:  ldc.i4.1
        IL_008f:  stloc.0
        IL_0090:  leave      IL_01e2

        IL_0095:  ldarg.0
        IL_0096:  ldc.i4.2
        IL_0097:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_009c:  ldarg.0
        IL_009d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1f'()
        IL_00a2:  ldarg.0
        IL_00a3:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
        IL_00a8:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00ad:  brtrue.s   IL_0063

        IL_00af:  ldarg.0
        IL_00b0:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1e'()
        IL_00b5:  ldarg.0
        IL_00b6:  ldstr      "A"
        IL_00bb:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_00c0:  ldarg.0
        IL_00c1:  ldc.i4.4
        IL_00c2:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00c7:  ldc.i4.1
        IL_00c8:  stloc.0
        IL_00c9:  leave      IL_01e2

        IL_00ce:  ldarg.0
        IL_00cf:  ldc.i4.m1
        IL_00d0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00d5:  ldarg.0
        IL_00d6:  ldstr      "B"
        IL_00db:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_00e0:  ldarg.0
        IL_00e1:  ldc.i4.5
        IL_00e2:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00e7:  ldc.i4.1
        IL_00e8:  stloc.0
        IL_00e9:  leave      IL_01e2

        IL_00ee:  ldarg.0
        IL_00ef:  ldc.i4.m1
        IL_00f0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00f5:  ldarg.0
        IL_00f6:  ldstr      "C"
        IL_00fb:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0100:  ldarg.0
        IL_0101:  ldc.i4.6
        IL_0102:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0107:  ldc.i4.1
        IL_0108:  stloc.0
        IL_0109:  leave      IL_01e2

        IL_010e:  ldarg.0
        IL_010f:  ldc.i4.m1
        IL_0110:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0115:  ldarg.0
        IL_0116:  ldstr      "D"
        IL_011b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0120:  ldarg.0
        IL_0121:  ldc.i4.7
        IL_0122:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0127:  ldc.i4.1
        IL_0128:  stloc.0
        IL_0129:  leave      IL_01e2

        IL_012e:  ldarg.0
        IL_012f:  ldc.i4.m1
        IL_0130:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0135:  ldarg.0
        IL_0136:  ldstr      "E"
        IL_013b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0140:  ldarg.0
        IL_0141:  ldc.i4.8
        IL_0142:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0147:  ldc.i4.1
        IL_0148:  stloc.0
        IL_0149:  leave      IL_01e2

        IL_014e:  ldarg.0
        IL_014f:  ldc.i4.m1
        IL_0150:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0155:  ldarg.0
        IL_0156:  ldstr      "F"
        IL_015b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0160:  ldarg.0
        IL_0161:  ldc.i4.s   9
        IL_0163:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0168:  ldc.i4.1
        IL_0169:  stloc.0
        IL_016a:  leave.s    IL_01e2

        IL_016c:  ldarg.0
        IL_016d:  ldc.i4.m1
        IL_016e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0173:  ldarg.0
        IL_0174:  ldarg.0
        IL_0175:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::input
        IL_017a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_017f:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
        IL_0184:  ldarg.0
        IL_0185:  ldc.i4.s   10
        IL_0187:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_018c:  br.s       IL_01c4

        IL_018e:  ldarg.0
        IL_018f:  ldarg.0
        IL_0190:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
        IL_0195:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_019a:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<item>5__1c'
        IL_019f:  ldarg.0
        IL_01a0:  ldarg.0
        IL_01a1:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<item>5__1c'
        IL_01a6:  callvirt   instance string [mscorlib]System.String::ToUpper()
        IL_01ab:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_01b0:  ldarg.0
        IL_01b1:  ldc.i4.s   11
        IL_01b3:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_01b8:  ldc.i4.1
        IL_01b9:  stloc.0
        IL_01ba:  leave.s    IL_01e2

        IL_01bc:  ldarg.0
        IL_01bd:  ldc.i4.s   10
        IL_01bf:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_01c4:  ldarg.0
        IL_01c5:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
        IL_01ca:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_01cf:  brtrue.s   IL_018e

        IL_01d1:  ldarg.0
        IL_01d2:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally21'()
        IL_01d7:  ldc.i4.0
        IL_01d8:  stloc.0
        IL_01d9:  leave.s    IL_01e2

      }  // end .try
      fault
      {
        IL_01db:  ldarg.0
        IL_01dc:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.IDisposable.Dispose()
        IL_01e1:  endfinally
      }  // end handler
      IL_01e2:  ldloc.0
      IL_01e3:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       107 (0x6b)
      .maxstack  2
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001d,
                            IL_001d,
                            IL_001d)
      IL_001b:  br.s       IL_0048

      .try
      {
        IL_001d:  ldarg.0
        IL_001e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0023:  stloc.1
        IL_0024:  ldloc.1
        IL_0025:  ldc.i4.2
        IL_0026:  sub
        IL_0027:  switch     ( 
                              IL_0036,
                              IL_0036)
        IL_0034:  br.s       IL_003f

        .try
        {
          IL_0036:  leave.s    IL_003f

        }  // end .try
        finally
        {
          IL_0038:  ldarg.0
          IL_0039:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1f'()
          IL_003e:  endfinally
        }  // end handler
        IL_003f:  leave.s    IL_0048

      }  // end .try
      finally
      {
        IL_0041:  ldarg.0
        IL_0042:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1e'()
        IL_0047:  endfinally
      }  // end handler
      IL_0048:  ldarg.0
      IL_0049:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_004e:  stloc.2
      IL_004f:  ldloc.2
      IL_0050:  ldc.i4.s   10
      IL_0052:  sub
      IL_0053:  switch     ( 
                            IL_0061,
                            IL_0061)
      IL_0060:  ret

      .try
      {
        IL_0061:  leave.s    IL_006a

      }  // end .try
      finally
      {
        IL_0063:  ldarg.0
        IL_0064:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally21'()
        IL_0069:  endfinally
      }  // end handler
      IL_006a:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally1e'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1e'

    .method private hidebysig instance void 
            '<>m__Finally1f'() cil managed
    {
      // Code size       29 (0x1d)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0007:  ldstr      "Processed "
      IL_000c:  ldarg.0
      IL_000d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<line>5__1b'
      IL_0012:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001c:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1f'

    .method private hidebysig instance void 
            '<>m__Finally21'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally21'

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass26'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string line
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass26'::.ctor

    .method public hidebysig instance string 
            '<YieldReturnWithAnonymousMethods1>b__24'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::line
      IL_0006:  ret
    } // end of method '<>c__DisplayClass26'::'<YieldReturnWithAnonymousMethods1>b__24'

  } // end of class '<>c__DisplayClass26'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithAnonymousMethods1>d__28'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class [mscorlib]System.Func`1<string> '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<>3__input'
    .field public class [mscorlib]System.Func`1<string> 'CS$<>9__CachedAnonymousMethodDelegate25'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' 'CS$<>8__locals27'
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap29'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> 
            'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>3__input'
      IL_0030:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::input
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       196 (0xc4)
      .maxstack  4
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001e,
                              IL_00b7,
                              IL_009d)
        IL_0019:  br         IL_00b7

        IL_001e:  ldarg.0
        IL_001f:  ldc.i4.m1
        IL_0020:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_0025:  ldarg.0
        IL_0026:  ldarg.0
        IL_0027:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::input
        IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.1
        IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_003d:  br.s       IL_00a4

        IL_003f:  ldarg.0
        IL_0040:  ldnull
        IL_0041:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_0046:  ldarg.0
        IL_0047:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::.ctor()
        IL_004c:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>8__locals27'
        IL_0051:  ldarg.0
        IL_0052:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>8__locals27'
        IL_0057:  ldarg.0
        IL_0058:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
        IL_005d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_0062:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::line
        IL_0067:  ldarg.0
        IL_0068:  ldarg.0
        IL_0069:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_006e:  brtrue.s   IL_0087

        IL_0070:  ldarg.0
        IL_0071:  ldarg.0
        IL_0072:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>8__locals27'
        IL_0077:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::'<YieldReturnWithAnonymousMethods1>b__24'()
        IL_007d:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_0082:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_0087:  ldarg.0
        IL_0088:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_008d:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>2__current'
        IL_0092:  ldarg.0
        IL_0093:  ldc.i4.2
        IL_0094:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_0099:  ldc.i4.1
        IL_009a:  stloc.0
        IL_009b:  leave.s    IL_00c2

        IL_009d:  ldarg.0
        IL_009e:  ldc.i4.1
        IL_009f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_00a4:  ldarg.0
        IL_00a5:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
        IL_00aa:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00af:  brtrue.s   IL_003f

        IL_00b1:  ldarg.0
        IL_00b2:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>m__Finally2a'()
        IL_00b7:  ldc.i4.0
        IL_00b8:  stloc.0
        IL_00b9:  leave.s    IL_00c2

      }  // end .try
      fault
      {
        IL_00bb:  ldarg.0
        IL_00bc:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::System.IDisposable.Dispose()
        IL_00c1:  endfinally
      }  // end handler
      IL_00c2:  ldloc.0
      IL_00c3:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance class [mscorlib]System.Func`1<string> 
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       34 (0x22)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0018,
                            IL_0018)
      IL_0017:  ret

      .try
      {
        IL_0018:  leave.s    IL_0021

      }  // end .try
      finally
      {
        IL_001a:  ldarg.0
        IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>m__Finally2a'()
        IL_0020:  endfinally
      }  // end handler
      IL_0021:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally2a'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::'<>m__Finally2a'

    .property instance class [mscorlib]System.Func`1<string>
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'()
    {
      .get instance class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'()
    } // end of property '<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithAnonymousMethods1>d__28'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithAnonymousMethods1>d__28'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2e'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string copy
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass2e'::.ctor

    .method public hidebysig instance string 
            '<YieldReturnWithAnonymousMethods2>b__2d'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::copy
      IL_0006:  ret
    } // end of method '<>c__DisplayClass2e'::'<YieldReturnWithAnonymousMethods2>b__2d'

  } // end of class '<>c__DisplayClass2e'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithAnonymousMethods2>d__30'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class [mscorlib]System.Func`1<string> '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<>3__input'
    .field public string '<item>5__31'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' 'CS$<>8__locals2f'
    .field public class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap32'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> 
            'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>3__input'
      IL_0030:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::input
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       181 (0xb5)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001e,
                              IL_00a8,
                              IL_008e)
        IL_0019:  br         IL_00a8

        IL_001e:  ldarg.0
        IL_001f:  ldc.i4.m1
        IL_0020:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_0025:  ldarg.0
        IL_0026:  ldarg.0
        IL_0027:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::input
        IL_002c:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0031:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.1
        IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_003d:  br.s       IL_0095

        IL_003f:  ldarg.0
        IL_0040:  ldarg.0
        IL_0041:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
        IL_0046:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_004b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<item>5__31'
        IL_0050:  ldarg.0
        IL_0051:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::.ctor()
        IL_0056:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'CS$<>8__locals2f'
        IL_005b:  ldarg.0
        IL_005c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'CS$<>8__locals2f'
        IL_0061:  ldarg.0
        IL_0062:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<item>5__31'
        IL_0067:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::copy
        IL_006c:  ldarg.0
        IL_006d:  ldarg.0
        IL_006e:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'CS$<>8__locals2f'
        IL_0073:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::'<YieldReturnWithAnonymousMethods2>b__2d'()
        IL_0079:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_007e:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>2__current'
        IL_0083:  ldarg.0
        IL_0084:  ldc.i4.2
        IL_0085:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_008a:  ldc.i4.1
        IL_008b:  stloc.0
        IL_008c:  leave.s    IL_00b3

        IL_008e:  ldarg.0
        IL_008f:  ldc.i4.1
        IL_0090:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_0095:  ldarg.0
        IL_0096:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
        IL_009b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00a0:  brtrue.s   IL_003f

        IL_00a2:  ldarg.0
        IL_00a3:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>m__Finally33'()
        IL_00a8:  ldc.i4.0
        IL_00a9:  stloc.0
        IL_00aa:  leave.s    IL_00b3

      }  // end .try
      fault
      {
        IL_00ac:  ldarg.0
        IL_00ad:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::System.IDisposable.Dispose()
        IL_00b2:  endfinally
      }  // end handler
      IL_00b3:  ldloc.0
      IL_00b4:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance class [mscorlib]System.Func`1<string> 
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       34 (0x22)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0018,
                            IL_0018)
      IL_0017:  ret

      .try
      {
        IL_0018:  leave.s    IL_0021

      }  // end .try
      finally
      {
        IL_001a:  ldarg.0
        IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>m__Finally33'()
        IL_0020:  endfinally
      }  // end handler
      IL_0021:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally33'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::'<>m__Finally33'

    .property instance class [mscorlib]System.Func`1<string>
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'()
    {
      .get instance class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'()
    } // end of property '<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithAnonymousMethods2>d__30'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithAnonymousMethods2>d__30'

  .class auto ansi sealed nested private beforefieldinit '<GetEvenNumbers>d__36'
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
    .field public int32 n
    .field public int32 '<>3__n'
    .field public int32 '<i>5__37'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>3__n'
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::n
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<GetEvenNumbers>d__36'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       107 (0x6b)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_0046)
      IL_0015:  br.s       IL_0069

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.m1
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  ldc.i4.0
      IL_0020:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0025:  br.s       IL_005b

      IL_0027:  ldarg.0
      IL_0028:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_002d:  ldc.i4.2
      IL_002e:  rem
      IL_002f:  brtrue.s   IL_004d

      IL_0031:  ldarg.0
      IL_0032:  ldarg.0
      IL_0033:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>2__current'
      IL_003d:  ldarg.0
      IL_003e:  ldc.i4.1
      IL_003f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0044:  ldc.i4.1
      IL_0045:  ret

      IL_0046:  ldarg.0
      IL_0047:  ldc.i4.m1
      IL_0048:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_004d:  ldarg.0
      IL_004e:  dup
      IL_004f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0054:  ldc.i4.1
      IL_0055:  add
      IL_0056:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_005b:  ldarg.0
      IL_005c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0061:  ldarg.0
      IL_0062:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::n
      IL_0067:  blt.s      IL_0027

      IL_0069:  ldc.i4.0
      IL_006a:  ret
    } // end of method '<GetEvenNumbers>d__36'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>2__current'
      IL_0006:  ret
    } // end of method '<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<GetEvenNumbers>d__36'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<GetEvenNumbers>d__36'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<GetEvenNumbers>d__36'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<GetEvenNumbers>d__36'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::System.Collections.IEnumerator.get_Current()
    } // end of property '<GetEvenNumbers>d__36'::System.Collections.IEnumerator.Current
  } // end of class '<GetEvenNumbers>d__36'

  .class auto ansi sealed nested private beforefieldinit '<ExceptionHandling>d__3a'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<char>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<char>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private char '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> 
            'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<char>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<ExceptionHandling>d__3a'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       208 (0xd0)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_002e,
                              IL_004b,
                              IL_00c3,
                              IL_007e,
                              IL_00c3,
                              IL_00c3,
                              IL_00b6)
        IL_0029:  br         IL_00c3

        IL_002e:  ldarg.0
        IL_002f:  ldc.i4.m1
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.s   97
        IL_0038:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
        IL_003d:  ldarg.0
        IL_003e:  ldc.i4.1
        IL_003f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_0044:  ldc.i4.1
        IL_0045:  stloc.0
        IL_0046:  leave      IL_00ce

        IL_004b:  ldarg.0
        IL_004c:  ldc.i4.m1
        IL_004d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        .try
        {
          IL_0052:  ldstr      "1 - try"
          IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005c:  leave.s    IL_006b

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_005e:  pop
          IL_005f:  ldstr      "1 - catch"
          IL_0064:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0069:  leave.s    IL_006b

        }  // end handler
        IL_006b:  ldarg.0
        IL_006c:  ldc.i4.s   98
        IL_006e:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.3
        IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_007a:  ldc.i4.1
        IL_007b:  stloc.0
        IL_007c:  leave.s    IL_00ce

        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.m1
        IL_0080:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_0085:  ldarg.0
        IL_0086:  ldc.i4.4
        IL_0087:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        .try
        {
          IL_008c:  ldstr      "2 - try"
          IL_0091:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0096:  leave.s    IL_00a3

        }  // end .try
        finally
        {
          IL_0098:  ldstr      "2 - finally"
          IL_009d:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00a2:  endfinally
        }  // end handler
        IL_00a3:  ldarg.0
        IL_00a4:  ldc.i4.s   99
        IL_00a6:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
        IL_00ab:  ldarg.0
        IL_00ac:  ldc.i4.6
        IL_00ad:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_00b2:  ldc.i4.1
        IL_00b3:  stloc.0
        IL_00b4:  leave.s    IL_00ce

        IL_00b6:  ldarg.0
        IL_00b7:  ldc.i4.4
        IL_00b8:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_00bd:  ldarg.0
        IL_00be:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>m__Finally3b'()
        IL_00c3:  ldc.i4.0
        IL_00c4:  stloc.0
        IL_00c5:  leave.s    IL_00ce

      }  // end .try
      fault
      {
        IL_00c7:  ldarg.0
        IL_00c8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::System.IDisposable.Dispose()
        IL_00cd:  endfinally
      }  // end handler
      IL_00ce:  ldloc.0
      IL_00cf:  ret
    } // end of method '<ExceptionHandling>d__3a'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance char  'System.Collections.Generic.IEnumerator<System.Char>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<char>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
      IL_0006:  ret
    } // end of method '<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<ExceptionHandling>d__3a'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       38 (0x26)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.4
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001c,
                            IL_0025,
                            IL_001c)
      IL_001b:  ret

      .try
      {
        IL_001c:  leave.s    IL_0025

      }  // end .try
      finally
      {
        IL_001e:  ldarg.0
        IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>m__Finally3b'()
        IL_0024:  endfinally
      }  // end handler
      IL_0025:  ret
    } // end of method '<ExceptionHandling>d__3a'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Char
      IL_000b:  ret
    } // end of method '<ExceptionHandling>d__3a'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<ExceptionHandling>d__3a'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally3b'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_0007:  ldstr      "outer finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<ExceptionHandling>d__3a'::'<>m__Finally3b'

    .property instance char 'System.Collections.Generic.IEnumerator<System.Char>.Current'()
    {
      .get instance char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'()
    } // end of property '<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerator<System.Char>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::System.Collections.IEnumerator.get_Current()
    } // end of property '<ExceptionHandling>d__3a'::System.Collections.IEnumerator.Current
  } // end of class '<ExceptionHandling>d__3a'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInCatch>d__3e'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       101 (0x65)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_0036,
                            IL_0063,
                            IL_005c)
      IL_001d:  br.s       IL_0063

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldc.i4.0
      IL_0028:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_002d:  ldarg.0
      IL_002e:  ldc.i4.1
      IL_002f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0034:  ldc.i4.1
      IL_0035:  ret

      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.m1
      IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      .try
      {
        IL_003d:  ldstr      "In Try"
        IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0047:  leave.s    IL_004c

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0049:  pop
        IL_004a:  leave.s    IL_0063

      }  // end handler
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.1
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0053:  ldarg.0
      IL_0054:  ldc.i4.3
      IL_0055:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_005a:  ldc.i4.1
      IL_005b:  ret

      IL_005c:  ldarg.0
      IL_005d:  ldc.i4.m1
      IL_005e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0063:  ldc.i4.0
      IL_0064:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInCatch>d__3e'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInCatch>d__3e'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInCatch>d__3e'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInCatchInTryFinally>d__41'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       139 (0x8b)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_007e,
                              IL_0043,
                              IL_007e,
                              IL_0071)
        IL_0021:  br.s       IL_007e

        IL_0023:  ldarg.0
        IL_0024:  ldc.i4.m1
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_002a:  ldarg.0
        IL_002b:  ldc.i4.1
        IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.0
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.2
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_003f:  ldc.i4.1
        IL_0040:  stloc.0
        IL_0041:  leave.s    IL_0089

        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.1
        IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        .try
        {
          IL_004a:  ldstr      "In Try"
          IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0054:  leave.s    IL_005f

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0056:  pop
          IL_0057:  ldarg.0
          IL_0058:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::System.IDisposable.Dispose()
          IL_005d:  leave.s    IL_007e

        }  // end handler
        IL_005f:  ldarg.0
        IL_0060:  ldc.i4.1
        IL_0061:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
        IL_0066:  ldarg.0
        IL_0067:  ldc.i4.4
        IL_0068:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_006d:  ldc.i4.1
        IL_006e:  stloc.0
        IL_006f:  leave.s    IL_0089

        IL_0071:  ldarg.0
        IL_0072:  ldc.i4.1
        IL_0073:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_0078:  ldarg.0
        IL_0079:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>m__Finally42'()
        IL_007e:  ldc.i4.0
        IL_007f:  stloc.0
        IL_0080:  leave.s    IL_0089

      }  // end .try
      fault
      {
        IL_0082:  ldarg.0
        IL_0083:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::System.IDisposable.Dispose()
        IL_0088:  endfinally
      }  // end handler
      IL_0089:  ldloc.0
      IL_008a:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       42 (0x2a)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0020,
                            IL_0020,
                            IL_0029,
                            IL_0020)
      IL_001f:  ret

      .try
      {
        IL_0020:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0022:  ldarg.0
        IL_0023:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>m__Finally42'()
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally42'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::'<>m__Finally42'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInCatchInTryFinally>d__41'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInCatchInTryFinally>d__41'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInTryCatchInTryFinally>d__45'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       149 (0x95)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_0088,
                              IL_0043,
                              IL_0088,
                              IL_007b)
        IL_0021:  br.s       IL_0088

        IL_0023:  ldarg.0
        IL_0024:  ldc.i4.m1
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_002a:  ldarg.0
        IL_002b:  ldc.i4.1
        IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.0
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.2
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_003f:  ldc.i4.1
        IL_0040:  stloc.0
        IL_0041:  leave.s    IL_0093

        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.1
        IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        .try
        {
          IL_004a:  ldstr      "In Try"
          IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0054:  ldarg.0
          IL_0055:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::System.IDisposable.Dispose()
          IL_005a:  leave.s    IL_0088

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_005c:  pop
          IL_005d:  ldstr      "Catch"
          IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0067:  leave.s    IL_0069

        }  // end handler
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.1
        IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
        IL_0070:  ldarg.0
        IL_0071:  ldc.i4.4
        IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0077:  ldc.i4.1
        IL_0078:  stloc.0
        IL_0079:  leave.s    IL_0093

        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.1
        IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0082:  ldarg.0
        IL_0083:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>m__Finally46'()
        IL_0088:  ldc.i4.0
        IL_0089:  stloc.0
        IL_008a:  leave.s    IL_0093

      }  // end .try
      fault
      {
        IL_008c:  ldarg.0
        IL_008d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::System.IDisposable.Dispose()
        IL_0092:  endfinally
      }  // end handler
      IL_0093:  ldloc.0
      IL_0094:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       42 (0x2a)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0020,
                            IL_0020,
                            IL_0029,
                            IL_0020)
      IL_001f:  ret

      .try
      {
        IL_0020:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0022:  ldarg.0
        IL_0023:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>m__Finally46'()
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally46'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::'<>m__Finally46'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInTryCatchInTryFinally>d__45'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInTryCatchInTryFinally>d__45'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInTryFinallyInTryFinally>d__49'
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
    .field public bool b
    .field public bool '<>3__b'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>3__b'
      IL_0030:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::b
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       157 (0x9d)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_0090,
                              IL_0043,
                              IL_0090,
                              IL_0083)
        IL_0021:  br.s       IL_0090

        IL_0023:  ldarg.0
        IL_0024:  ldc.i4.m1
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_002a:  ldarg.0
        IL_002b:  ldc.i4.1
        IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.0
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.2
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_003f:  ldc.i4.1
        IL_0040:  stloc.0
        IL_0041:  leave.s    IL_009b

        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.1
        IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        .try
        {
          IL_004a:  ldstr      "In Try"
          IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0054:  ldarg.0
          IL_0055:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::b
          IL_005a:  brfalse.s  IL_0064

          IL_005c:  ldarg.0
          IL_005d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::System.IDisposable.Dispose()
          IL_0062:  leave.s    IL_0090

          IL_0064:  leave.s    IL_0071

        }  // end .try
        finally
        {
          IL_0066:  ldstr      "Inner Finally"
          IL_006b:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0070:  endfinally
        }  // end handler
        IL_0071:  ldarg.0
        IL_0072:  ldc.i4.1
        IL_0073:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
        IL_0078:  ldarg.0
        IL_0079:  ldc.i4.4
        IL_007a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_007f:  ldc.i4.1
        IL_0080:  stloc.0
        IL_0081:  leave.s    IL_009b

        IL_0083:  ldarg.0
        IL_0084:  ldc.i4.1
        IL_0085:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_008a:  ldarg.0
        IL_008b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>m__Finally4a'()
        IL_0090:  ldc.i4.0
        IL_0091:  stloc.0
        IL_0092:  leave.s    IL_009b

      }  // end .try
      fault
      {
        IL_0094:  ldarg.0
        IL_0095:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::System.IDisposable.Dispose()
        IL_009a:  endfinally
      }  // end handler
      IL_009b:  ldloc.0
      IL_009c:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       42 (0x2a)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0020,
                            IL_0020,
                            IL_0029,
                            IL_0020)
      IL_001f:  ret

      .try
      {
        IL_0020:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0022:  ldarg.0
        IL_0023:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>m__Finally4a'()
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally4a'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::'<>m__Finally4a'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInTryFinallyInTryFinally>d__49'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInTryFinallyInTryFinally>d__49'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakOnly>d__4d'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakOnly>d__4d'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       20 (0x14)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.0
      IL_0009:  bne.un.s   IL_0012

      IL_000b:  ldarg.0
      IL_000c:  ldc.i4.m1
      IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method '<YieldBreakOnly>d__4d'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakOnly>d__4d'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldBreakOnly>d__4d'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakOnly>d__4d'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakOnly>d__4d'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakOnly>d__4d'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakOnly>d__4d'

  .class auto ansi sealed nested private beforefieldinit '<UnconditionalThrowInTryFinally>d__50'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       85 (0x55)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_0048,
                              IL_003b)
        IL_0019:  br.s       IL_0048

        IL_001b:  ldarg.0
        IL_001c:  ldc.i4.m1
        IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0022:  ldarg.0
        IL_0023:  ldc.i4.1
        IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0029:  ldarg.0
        IL_002a:  ldc.i4.0
        IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>2__current'
        IL_0030:  ldarg.0
        IL_0031:  ldc.i4.2
        IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0037:  ldc.i4.1
        IL_0038:  stloc.0
        IL_0039:  leave.s    IL_0053

        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.1
        IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0042:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
        IL_0047:  throw

        IL_0048:  ldc.i4.0
        IL_0049:  stloc.0
        IL_004a:  leave.s    IL_0053

      }  // end .try
      fault
      {
        IL_004c:  ldarg.0
        IL_004d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::System.IDisposable.Dispose()
        IL_0052:  endfinally
      }  // end handler
      IL_0053:  ldloc.0
      IL_0054:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>2__current'
      IL_0006:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       34 (0x22)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0018,
                            IL_0018)
      IL_0017:  ret

      .try
      {
        IL_0018:  leave.s    IL_0021

      }  // end .try
      finally
      {
        IL_001a:  ldarg.0
        IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>m__Finally51'()
        IL_0020:  endfinally
      }  // end handler
      IL_0021:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally51'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::'<>m__Finally51'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::System.Collections.IEnumerator.get_Current()
    } // end of property '<UnconditionalThrowInTryFinally>d__50'::System.Collections.IEnumerator.Current
  } // end of class '<UnconditionalThrowInTryFinally>d__50'

  .class auto ansi sealed nested private beforefieldinit '<NestedTryFinallyStartingOnSamePosition>d__54'
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
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       88 (0x58)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  ldc.i4.0
        IL_0009:  beq.s      IL_0011

        IL_000b:  ldloc.1
        IL_000c:  ldc.i4.3
        IL_000d:  beq.s      IL_0038

        IL_000f:  br.s       IL_004b

        IL_0011:  ldarg.0
        IL_0012:  ldc.i4.m1
        IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0018:  ldarg.0
        IL_0019:  ldc.i4.1
        IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_001f:  ldarg.0
        IL_0020:  ldc.i4.2
        IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0026:  ldarg.0
        IL_0027:  ldc.i4.0
        IL_0028:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>2__current'
        IL_002d:  ldarg.0
        IL_002e:  ldc.i4.3
        IL_002f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0034:  ldc.i4.1
        IL_0035:  stloc.0
        IL_0036:  leave.s    IL_0056

        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.2
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_003f:  ldarg.0
        IL_0040:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally56'()
        IL_0045:  ldarg.0
        IL_0046:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally55'()
        IL_004b:  ldc.i4.0
        IL_004c:  stloc.0
        IL_004d:  leave.s    IL_0056

      }  // end .try
      fault
      {
        IL_004f:  ldarg.0
        IL_0050:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::System.IDisposable.Dispose()
        IL_0055:  endfinally
      }  // end handler
      IL_0056:  ldloc.0
      IL_0057:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>2__current'
      IL_0006:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       72 (0x48)
      .maxstack  2
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001c,
                            IL_001c,
                            IL_001c)
      IL_001b:  ret

      .try
      {
        IL_001c:  ldarg.0
        IL_001d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0022:  stloc.1
        IL_0023:  ldloc.1
        IL_0024:  ldc.i4.2
        IL_0025:  sub
        IL_0026:  switch     ( 
                              IL_0035,
                              IL_0035)
        IL_0033:  br.s       IL_003e

        .try
        {
          IL_0035:  leave.s    IL_003e

        }  // end .try
        finally
        {
          IL_0037:  ldarg.0
          IL_0038:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally56'()
          IL_003d:  endfinally
        }  // end handler
        IL_003e:  leave.s    IL_0047

      }  // end .try
      finally
      {
        IL_0040:  ldarg.0
        IL_0041:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally55'()
        IL_0046:  endfinally
      }  // end handler
      IL_0047:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally55'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0007:  ldstr      "Outer Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally55'

    .method private hidebysig instance void 
            '<>m__Finally56'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0007:  ldstr      "Inner Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally56'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::System.Collections.IEnumerator.get_Current()
    } // end of property '<NestedTryFinallyStartingOnSamePosition>d__54'::System.Collections.IEnumerator.Current
  } // end of class '<NestedTryFinallyStartingOnSamePosition>d__54'

  .class auto ansi sealed nested private beforefieldinit '<LocalInFinally>d__59`1'<([mscorlib]System.IDisposable) T>
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
    .field public !T a
    .field public !T '<>3__a'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T> V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>3__a'
      IL_0030:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::a
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<LocalInFinally>d__59`1'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<LocalInFinally>d__59`1'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       143 (0x8f)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_003c,
                              IL_0082,
                              IL_005c,
                              IL_007b)
        IL_0021:  br.s       IL_0082

        IL_0023:  ldarg.0
        IL_0024:  ldc.i4.m1
        IL_0025:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_002a:  ldarg.0
        IL_002b:  ldc.i4.1
        IL_002c:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.1
        IL_0033:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0038:  ldc.i4.1
        IL_0039:  stloc.0
        IL_003a:  leave.s    IL_008d

        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.m1
        IL_003e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.2
        IL_0045:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_004a:  ldarg.0
        IL_004b:  ldc.i4.2
        IL_004c:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
        IL_0051:  ldarg.0
        IL_0052:  ldc.i4.3
        IL_0053:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0058:  ldc.i4.1
        IL_0059:  stloc.0
        IL_005a:  leave.s    IL_008d

        IL_005c:  ldarg.0
        IL_005d:  ldc.i4.2
        IL_005e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0063:  ldarg.0
        IL_0064:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>m__Finally5a'()
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.3
        IL_006b:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
        IL_0070:  ldarg.0
        IL_0071:  ldc.i4.4
        IL_0072:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0077:  ldc.i4.1
        IL_0078:  stloc.0
        IL_0079:  leave.s    IL_008d

        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.m1
        IL_007d:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0082:  ldc.i4.0
        IL_0083:  stloc.0
        IL_0084:  leave.s    IL_008d

      }  // end .try
      fault
      {
        IL_0086:  ldarg.0
        IL_0087:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::System.IDisposable.Dispose()
        IL_008c:  endfinally
      }  // end handler
      IL_008d:  ldloc.0
      IL_008e:  ret
    } // end of method '<LocalInFinally>d__59`1'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
      IL_0006:  ret
    } // end of method '<LocalInFinally>d__59`1'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<LocalInFinally>d__59`1'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       34 (0x22)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.2
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0018,
                            IL_0018)
      IL_0017:  ret

      .try
      {
        IL_0018:  leave.s    IL_0021

      }  // end .try
      finally
      {
        IL_001a:  ldarg.0
        IL_001b:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>m__Finally5a'()
        IL_0020:  endfinally
      }  // end handler
      IL_0021:  ret
    } // end of method '<LocalInFinally>d__59`1'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<LocalInFinally>d__59`1'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<LocalInFinally>d__59`1'::.ctor

    .method private hidebysig instance void 
            '<>m__Finally5a'() cil managed
    {
      // Code size       41 (0x29)
      .maxstack  2
      .locals init (!T V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::a
      IL_000d:  stloc.0
      IL_000e:  ldloca.s   V_0
      IL_0010:  constrained. !T
      IL_0016:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001b:  ldloca.s   V_0
      IL_001d:  constrained. !T
      IL_0023:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0028:  ret
    } // end of method '<LocalInFinally>d__59`1'::'<>m__Finally5a'

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<LocalInFinally>d__59`1'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'::System.Collections.IEnumerator.get_Current()
    } // end of property '<LocalInFinally>d__59`1'::System.Collections.IEnumerator.Current
  } // end of class '<LocalInFinally>d__59`1'

  .class auto ansi sealed nested private beforefieldinit '<GenericYield>d__5d`1'<.ctor T>
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>,
                    [mscorlib]System.Collections.IEnumerator,
                    [mscorlib]System.IDisposable
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private !T '<>2__current'
    .field private int32 '<>1__state'
    .field private int32 '<>l__initialThreadId'
    .field public !T '<val>5__5e'
    .field public int32 '<i>5__5f'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> 
            'System.Collections.Generic.IEnumerable<T>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T> V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<GenericYield>d__5d`1'::'System.Collections.Generic.IEnumerable<T>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'System.Collections.Generic.IEnumerable<T>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<GenericYield>d__5d`1'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       130 (0x82)
      .maxstack  3
      .locals init (int32 V_0,
               !T V_1,
               !T V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_0062)
      IL_0015:  br.s       IL_0080

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.m1
      IL_0019:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  ldloca.s   V_1
      IL_0021:  initobj    !T
      IL_0027:  ldloc.1
      IL_0028:  box        !T
      IL_002d:  brfalse.s  IL_003a

      IL_002f:  ldloca.s   V_2
      IL_0031:  initobj    !T
      IL_0037:  ldloc.2
      IL_0038:  br.s       IL_003f

      IL_003a:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
      IL_003f:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<val>5__5e'
      IL_0044:  ldarg.0
      IL_0045:  ldc.i4.0
      IL_0046:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_004b:  br.s       IL_0077

      IL_004d:  ldarg.0
      IL_004e:  ldarg.0
      IL_004f:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<val>5__5e'
      IL_0054:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>2__current'
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.1
      IL_005b:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0060:  ldc.i4.1
      IL_0061:  ret

      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.m1
      IL_0064:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0069:  ldarg.0
      IL_006a:  dup
      IL_006b:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_0070:  ldc.i4.1
      IL_0071:  add
      IL_0072:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_0077:  ldarg.0
      IL_0078:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_007d:  ldc.i4.3
      IL_007e:  blt.s      IL_004d

      IL_0080:  ldc.i4.0
      IL_0081:  ret
    } // end of method '<GenericYield>d__5d`1'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance !T  'System.Collections.Generic.IEnumerator<T>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>2__current'
      IL_0006:  ret
    } // end of method '<GenericYield>d__5d`1'::'System.Collections.Generic.IEnumerator<T>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<GenericYield>d__5d`1'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<GenericYield>d__5d`1'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>2__current'
      IL_0006:  box        !T
      IL_000b:  ret
    } // end of method '<GenericYield>d__5d`1'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<GenericYield>d__5d`1'::.ctor

    .property instance !T 'System.Collections.Generic.IEnumerator<T>.Current'()
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'::'System.Collections.Generic.IEnumerator<T>.get_Current'()
    } // end of property '<GenericYield>d__5d`1'::'System.Collections.Generic.IEnumerator<T>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'::System.Collections.IEnumerator.get_Current()
    } // end of property '<GenericYield>d__5d`1'::System.Collections.IEnumerator.Current
  } // end of class '<GenericYield>d__5d`1'

  .field private int32 fieldOnThis
  .method public hidebysig specialname static 
          class [mscorlib]System.Collections.Generic.IEnumerable`1<char> 
          get_YieldChars() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::get_YieldChars

  .method assembly hidebysig static void 
          Print<T>(string name,
                   class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T> enumerator) cil managed
  {
    // Code size       54 (0x36)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldstr      ": Test start"
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  br.s       IL_002d

    IL_0012:  ldarg.0
    IL_0013:  ldstr      ": "
    IL_0018:  ldarg.1
    IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T>::get_Current()
    IL_001e:  box        !!T
    IL_0023:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002d:  ldarg.1
    IL_002e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
    IL_0033:  brtrue.s   IL_0012

    IL_0035:  ret
  } // end of method YieldReturnPrettyTest::Print

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          SimpleYieldReturn() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::SimpleYieldReturn

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
          SimpleYieldReturnEnumerator() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6' V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method YieldReturnPrettyTest::SimpleYieldReturnEnumerator

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnParameters(int32 p) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
    IL_000f:  ldloc.0
    IL_0010:  ldarg.1
    IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>3__p'
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnParameters

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
          YieldReturnParametersEnumerator(int32 p) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b' V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.0
    IL_0009:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>4__this'
    IL_000e:  ldloc.0
    IL_000f:  ldarg.1
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::p
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnParametersEnumerator

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnInLoop() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnInLoop

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnWithTryFinally() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          YieldReturnWithNestedTryFinally(bool breakInMiddle) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>3__breakInMiddle'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithNestedTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          YieldReturnWithTwoNonNestedFinallyBlocks(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>3__input'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithTwoNonNestedFinallyBlocks

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> 
          YieldReturnWithAnonymousMethods1(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>3__input'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithAnonymousMethods1

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> 
          YieldReturnWithAnonymousMethods2(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>3__input'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithAnonymousMethods2

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          GetEvenNumbers(int32 n) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>3__n'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::GetEvenNumbers

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<char> 
          ExceptionHandling() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::ExceptionHandling

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInCatch() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInCatch

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInCatchInTryFinally() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInCatchInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInTryCatchInTryFinally() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInTryCatchInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInTryFinallyInTryFinally(bool b) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>3__b'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInTryFinallyInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakOnly() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakOnly

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          UnconditionalThrowInTryFinally() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::UnconditionalThrowInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          NestedTryFinallyStartingOnSamePosition() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::NestedTryFinallyStartingOnSamePosition

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          LocalInFinally<([mscorlib]System.IDisposable) T>(!!T a) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!!T> V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!!T>::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!!T>::'<>3__a'
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method YieldReturnPrettyTest::LocalInFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T> 
          GenericYield<.ctor T>() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!!T> V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!!T>::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method YieldReturnPrettyTest::GenericYield

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method YieldReturnPrettyTest::.ctor

  .property class [mscorlib]System.Collections.Generic.IEnumerable`1<char>
          YieldChars()
  {
    .get class [mscorlib]System.Collections.Generic.IEnumerable`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::get_YieldChars()
  } // end of property YieldReturnPrettyTest::YieldChars
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest

.class private sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn
       extends [mscorlib]System.ValueType
{
  .class auto ansi sealed nested private beforefieldinit '<Count>d__0'
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
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn '<>4__this'
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn '<>3__<>4__this'
    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0' V_0)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_0022

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0013:  ldc.i4.s   -2
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0030:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>3__<>4__this'
      IL_003c:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0041:  ldloc.0
      IL_0042:  ret
    } // end of method '<Count>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<Count>d__0'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       124 (0x7c)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_0047,
                            IL_0073)
      IL_0019:  br.s       IL_007a

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.m1
      IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0022:  ldarg.0
      IL_0023:  ldarg.0
      IL_0024:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0029:  dup
      IL_002a:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_002f:  dup
      IL_0030:  stloc.1
      IL_0031:  ldc.i4.1
      IL_0032:  add
      IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0038:  ldloc.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.1
      IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0045:  ldc.i4.1
      IL_0046:  ret

      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.m1
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_004e:  ldarg.0
      IL_004f:  ldarg.0
      IL_0050:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0055:  dup
      IL_0056:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_005b:  dup
      IL_005c:  stloc.2
      IL_005d:  ldc.i4.1
      IL_005e:  add
      IL_005f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0064:  ldloc.2
      IL_0065:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_006a:  ldarg.0
      IL_006b:  ldc.i4.2
      IL_006c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0071:  ldc.i4.1
      IL_0072:  ret

      IL_0073:  ldarg.0
      IL_0074:  ldc.i4.m1
      IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_007a:  ldc.i4.0
      IL_007b:  ret
    } // end of method '<Count>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_0006:  ret
    } // end of method '<Count>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<Count>d__0'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Count>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<Count>d__0'::System.Collections.IEnumerator.get_Current

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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<Count>d__0'::.ctor

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<Count>d__0'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::System.Collections.IEnumerator.get_Current()
    } // end of property '<Count>d__0'::System.Collections.IEnumerator.Current
  } // end of class '<Count>d__0'

  .field private int32 val
  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          Count() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0' V_0)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn
    IL_000f:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>3__<>4__this'
    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method StructWithYieldReturn::Count

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
