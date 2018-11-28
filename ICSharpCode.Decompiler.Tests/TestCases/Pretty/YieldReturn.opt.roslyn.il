



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly YieldReturn
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
.module YieldReturn.dll
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
  .class auto ansi sealed nested private beforefieldinit '<get_YieldChars>d__2'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<char>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<char>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private char '<>2__current'
    .field private int32 '<>l__initialThreadId'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<get_YieldChars>d__2'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<get_YieldChars>d__2'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       112 (0x70)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_0037,
                            IL_004f,
                            IL_0067)
      IL_001d:  ldc.i4.0
      IL_001e:  ret

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldc.i4.s   97
      IL_0029:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_002e:  ldarg.0
      IL_002f:  ldc.i4.1
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0035:  ldc.i4.1
      IL_0036:  ret

      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.m1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.s   98
      IL_0041:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_0046:  ldarg.0
      IL_0047:  ldc.i4.2
      IL_0048:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_004d:  ldc.i4.1
      IL_004e:  ret

      IL_004f:  ldarg.0
      IL_0050:  ldc.i4.m1
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0056:  ldarg.0
      IL_0057:  ldc.i4.s   99
      IL_0059:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_005e:  ldarg.0
      IL_005f:  ldc.i4.3
      IL_0060:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0065:  ldc.i4.1
      IL_0066:  ret

      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.m1
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_006e:  ldc.i4.0
      IL_006f:  ret
    } // end of method '<get_YieldChars>d__2'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance char  'System.Collections.Generic.IEnumerator<System.Char>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<char>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_0006:  ret
    } // end of method '<get_YieldChars>d__2'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<get_YieldChars>d__2'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Char
      IL_000b:  ret
    } // end of method '<get_YieldChars>d__2'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> 
            'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<char>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<get_YieldChars>d__2'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<get_YieldChars>d__2'::System.Collections.IEnumerable.GetEnumerator

    .property instance char 'System.Collections.Generic.IEnumerator<System.Char>.Current'()
    {
      .get instance char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'()
    } // end of property '<get_YieldChars>d__2'::'System.Collections.Generic.IEnumerator<System.Char>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::System.Collections.IEnumerator.get_Current()
    } // end of property '<get_YieldChars>d__2'::System.Collections.IEnumerator.Current
  } // end of class '<get_YieldChars>d__2'

  .class auto ansi sealed nested private beforefieldinit '<SimpleYieldReturn>d__4'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<SimpleYieldReturn>d__4'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleYieldReturn>d__4'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       121 (0x79)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_003a,
                            IL_0055,
                            IL_0070)
      IL_001d:  ldc.i4.0
      IL_001e:  ret

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldstr      "A"
      IL_002c:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_0031:  ldarg.0
      IL_0032:  ldc.i4.1
      IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0038:  ldc.i4.1
      IL_0039:  ret

      IL_003a:  ldarg.0
      IL_003b:  ldc.i4.m1
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0041:  ldarg.0
      IL_0042:  ldstr      "B"
      IL_0047:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.2
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0053:  ldc.i4.1
      IL_0054:  ret

      IL_0055:  ldarg.0
      IL_0056:  ldc.i4.m1
      IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_005c:  ldarg.0
      IL_005d:  ldstr      "C"
      IL_0062:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.3
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_006e:  ldc.i4.1
      IL_006f:  ret

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.m1
      IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0077:  ldc.i4.0
      IL_0078:  ret
    } // end of method '<SimpleYieldReturn>d__4'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturn>d__4'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<SimpleYieldReturn>d__4'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturn>d__4'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<SimpleYieldReturn>d__4'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<SimpleYieldReturn>d__4'::System.Collections.IEnumerable.GetEnumerator

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<SimpleYieldReturn>d__4'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::System.Collections.IEnumerator.get_Current()
    } // end of property '<SimpleYieldReturn>d__4'::System.Collections.IEnumerator.Current
  } // end of class '<SimpleYieldReturn>d__4'

  .class auto ansi sealed nested private beforefieldinit '<SimpleYieldReturnEnumerator>d__5'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerator`1<string>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private string '<>2__current'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_000d:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__5'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__5'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       121 (0x79)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_003a,
                            IL_0055,
                            IL_0070)
      IL_001d:  ldc.i4.0
      IL_001e:  ret

      IL_001f:  ldarg.0
      IL_0020:  ldc.i4.m1
      IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0026:  ldarg.0
      IL_0027:  ldstr      "A"
      IL_002c:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_0031:  ldarg.0
      IL_0032:  ldc.i4.1
      IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0038:  ldc.i4.1
      IL_0039:  ret

      IL_003a:  ldarg.0
      IL_003b:  ldc.i4.m1
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0041:  ldarg.0
      IL_0042:  ldstr      "B"
      IL_0047:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.2
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0053:  ldc.i4.1
      IL_0054:  ret

      IL_0055:  ldarg.0
      IL_0056:  ldc.i4.m1
      IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_005c:  ldarg.0
      IL_005d:  ldstr      "C"
      IL_0062:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.3
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_006e:  ldc.i4.1
      IL_006f:  ret

      IL_0070:  ldarg.0
      IL_0071:  ldc.i4.m1
      IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0077:  ldc.i4.0
      IL_0078:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__5'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__5'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<SimpleYieldReturnEnumerator>d__5'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_0006:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__5'::System.Collections.IEnumerator.get_Current

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<SimpleYieldReturnEnumerator>d__5'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::System.Collections.IEnumerator.get_Current()
    } // end of property '<SimpleYieldReturnEnumerator>d__5'::System.Collections.IEnumerator.Current
  } // end of class '<SimpleYieldReturnEnumerator>d__5'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnParameters>d__6'
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
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest '<>4__this'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnParameters>d__6'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldReturnParameters>d__6'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       99 (0x63)
      .maxstack  2
      .locals init (int32 V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>4__this'
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  switch     ( 
                            IL_0022,
                            IL_003e,
                            IL_005a)
      IL_0020:  ldc.i4.0
      IL_0021:  ret

      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.m1
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::p
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>2__current'
      IL_0035:  ldarg.0
      IL_0036:  ldc.i4.1
      IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_003c:  ldc.i4.1
      IL_003d:  ret

      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.m1
      IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0045:  ldarg.0
      IL_0046:  ldloc.1
      IL_0047:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_004c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>2__current'
      IL_0051:  ldarg.0
      IL_0052:  ldc.i4.2
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0058:  ldc.i4.1
      IL_0059:  ret

      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.m1
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0061:  ldc.i4.0
      IL_0062:  ret
    } // end of method '<YieldReturnParameters>d__6'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnParameters>d__6'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnParameters>d__6'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnParameters>d__6'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0035

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>4__this'
      IL_0030:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>3__p'
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::p
      IL_0041:  ldloc.0
      IL_0042:  ret
    } // end of method '<YieldReturnParameters>d__6'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnParameters>d__6'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnParameters>d__6'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnParameters>d__6'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnParameters>d__6'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnParametersEnumerator>d__7'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private int32 '<>2__current'
    .field public int32 p
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest '<>4__this'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_000d:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__7'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__7'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       99 (0x63)
      .maxstack  2
      .locals init (int32 V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>4__this'
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  switch     ( 
                            IL_0022,
                            IL_003e,
                            IL_005a)
      IL_0020:  ldc.i4.0
      IL_0021:  ret

      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.m1
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::p
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>2__current'
      IL_0035:  ldarg.0
      IL_0036:  ldc.i4.1
      IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_003c:  ldc.i4.1
      IL_003d:  ret

      IL_003e:  ldarg.0
      IL_003f:  ldc.i4.m1
      IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0045:  ldarg.0
      IL_0046:  ldloc.1
      IL_0047:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_004c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>2__current'
      IL_0051:  ldarg.0
      IL_0052:  ldc.i4.2
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0058:  ldc.i4.1
      IL_0059:  ret

      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.m1
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0061:  ldc.i4.0
      IL_0062:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__7'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__7'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnParametersEnumerator>d__7'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__7'::System.Collections.IEnumerator.get_Current

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnParametersEnumerator>d__7'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnParametersEnumerator>d__7'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnParametersEnumerator>d__7'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnInLoop>d__8'
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
    .field private int32 '<i>5__2'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnInLoop>d__8'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldReturnInLoop>d__8'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       88 (0x58)
      .maxstack  3
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0010

      IL_000a:  ldloc.0
      IL_000b:  ldc.i4.1
      IL_000c:  beq.s      IL_0035

      IL_000e:  ldc.i4.0
      IL_000f:  ret

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.m1
      IL_0012:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__2'
      IL_001e:  br.s       IL_004c

      IL_0020:  ldarg.0
      IL_0021:  ldarg.0
      IL_0022:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__2'
      IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>2__current'
      IL_002c:  ldarg.0
      IL_002d:  ldc.i4.1
      IL_002e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_0033:  ldc.i4.1
      IL_0034:  ret

      IL_0035:  ldarg.0
      IL_0036:  ldc.i4.m1
      IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_003c:  ldarg.0
      IL_003d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__2'
      IL_0042:  stloc.1
      IL_0043:  ldarg.0
      IL_0044:  ldloc.1
      IL_0045:  ldc.i4.1
      IL_0046:  add
      IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__2'
      IL_004c:  ldarg.0
      IL_004d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__2'
      IL_0052:  ldc.i4.s   100
      IL_0054:  blt.s      IL_0020

      IL_0056:  ldc.i4.0
      IL_0057:  ret
    } // end of method '<YieldReturnInLoop>d__8'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnInLoop>d__8'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnInLoop>d__8'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnInLoop>d__8'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldReturnInLoop>d__8'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnInLoop>d__8'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnInLoop>d__8'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnInLoop>d__8'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnInLoop>d__8'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithTryFinally>d__9'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0010

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.2
      IL_000e:  bne.un.s   IL_001a

      IL_0010:  nop
      .try
      {
        IL_0011:  leave.s    IL_001a

      }  // end .try
      finally
      {
        IL_0013:  ldarg.0
        IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>m__Finally1'()
        IL_0019:  endfinally
      }  // end handler
      IL_001a:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::System.IDisposable.Dispose

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
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0021,
                              IL_003a,
                              IL_005b,
                              IL_007b)
        IL_001d:  ldc.i4.0
        IL_001e:  stloc.0
        IL_001f:  leave.s    IL_008d

        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.m1
        IL_0023:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.0
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
        IL_002f:  ldarg.0
        IL_0030:  ldc.i4.1
        IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0036:  ldc.i4.1
        IL_0037:  stloc.0
        IL_0038:  leave.s    IL_008d

        IL_003a:  ldarg.0
        IL_003b:  ldc.i4.m1
        IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.s   -3
        IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0049:  ldarg.0
        IL_004a:  ldc.i4.1
        IL_004b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
        IL_0050:  ldarg.0
        IL_0051:  ldc.i4.2
        IL_0052:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0057:  ldc.i4.1
        IL_0058:  stloc.0
        IL_0059:  leave.s    IL_008d

        IL_005b:  ldarg.0
        IL_005c:  ldc.i4.s   -3
        IL_005e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0063:  ldarg.0
        IL_0064:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>m__Finally1'()
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.2
        IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
        IL_0070:  ldarg.0
        IL_0071:  ldc.i4.3
        IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0077:  ldc.i4.1
        IL_0078:  stloc.0
        IL_0079:  leave.s    IL_008d

        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.m1
        IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0082:  ldc.i4.0
        IL_0083:  stloc.0
        IL_0084:  leave.s    IL_008d

      }  // end .try
      fault
      {
        IL_0086:  ldarg.0
        IL_0087:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::System.IDisposable.Dispose()
        IL_008c:  endfinally
      }  // end handler
      IL_008d:  ldloc.0
      IL_008e:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_0007:  ldstr      "Finally!"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithTryFinally>d__9'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldReturnWithTryFinally>d__9'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithTryFinally>d__9'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithTryFinally>d__9'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithNestedTryFinally>d__10'
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
    .field private bool breakInMiddle
    .field public bool '<>3__breakInMiddle'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       52 (0x34)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -4
      IL_000a:  sub
      IL_000b:  ldc.i4.1
      IL_000c:  ble.un.s   IL_0014

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.2
      IL_0010:  sub
      IL_0011:  ldc.i4.3
      IL_0012:  bgt.un.s   IL_0033

      IL_0014:  nop
      .try
      {
        IL_0015:  ldloc.0
        IL_0016:  ldc.i4.s   -4
        IL_0018:  beq.s      IL_0022

        IL_001a:  ldloc.0
        IL_001b:  ldc.i4.3
        IL_001c:  sub
        IL_001d:  ldc.i4.1
        IL_001e:  ble.un.s   IL_0022

        IL_0020:  leave.s    IL_0033

        IL_0022:  nop
        .try
        {
          IL_0023:  leave.s    IL_0033

        }  // end .try
        finally
        {
          IL_0025:  ldarg.0
          IL_0026:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'()
          IL_002b:  endfinally
        }  // end handler
      }  // end .try
      finally
      {
        IL_002c:  ldarg.0
        IL_002d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'()
        IL_0032:  endfinally
      }  // end handler
      IL_0033:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       448 (0x1c0)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0030,
                              IL_005a,
                              IL_0096,
                              IL_00d3,
                              IL_011e,
                              IL_0160,
                              IL_01a2)
        IL_0029:  ldc.i4.0
        IL_002a:  stloc.0
        IL_002b:  leave      IL_01be

        IL_0030:  ldarg.0
        IL_0031:  ldc.i4.m1
        IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0037:  ldstr      "Start of method - 1"
        IL_003c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0041:  ldarg.0
        IL_0042:  ldstr      "Start of method"
        IL_0047:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_004c:  ldarg.0
        IL_004d:  ldc.i4.1
        IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0053:  ldc.i4.1
        IL_0054:  stloc.0
        IL_0055:  leave      IL_01be

        IL_005a:  ldarg.0
        IL_005b:  ldc.i4.m1
        IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0061:  ldstr      "Start of method - 2"
        IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_006b:  ldarg.0
        IL_006c:  ldc.i4.s   -3
        IL_006e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0073:  ldstr      "Within outer try - 1"
        IL_0078:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_007d:  ldarg.0
        IL_007e:  ldstr      "Within outer try"
        IL_0083:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_0088:  ldarg.0
        IL_0089:  ldc.i4.2
        IL_008a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_008f:  ldc.i4.1
        IL_0090:  stloc.0
        IL_0091:  leave      IL_01be

        IL_0096:  ldarg.0
        IL_0097:  ldc.i4.s   -3
        IL_0099:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_009e:  ldstr      "Within outer try - 2"
        IL_00a3:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00a8:  ldarg.0
        IL_00a9:  ldc.i4.s   -4
        IL_00ab:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00b0:  ldstr      "Within inner try - 1"
        IL_00b5:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00ba:  ldarg.0
        IL_00bb:  ldstr      "Within inner try"
        IL_00c0:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_00c5:  ldarg.0
        IL_00c6:  ldc.i4.3
        IL_00c7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00cc:  ldc.i4.1
        IL_00cd:  stloc.0
        IL_00ce:  leave      IL_01be

        IL_00d3:  ldarg.0
        IL_00d4:  ldc.i4.s   -4
        IL_00d6:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00db:  ldstr      "Within inner try - 2"
        IL_00e0:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00e5:  ldarg.0
        IL_00e6:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::breakInMiddle
        IL_00eb:  brfalse.s  IL_00fb

        IL_00ed:  ldstr      "Breaking..."
        IL_00f2:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00f7:  ldc.i4.0
        IL_00f8:  stloc.0
        IL_00f9:  br.s       IL_0138

        IL_00fb:  ldstr      "End of inner try - 1"
        IL_0100:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0105:  ldarg.0
        IL_0106:  ldstr      "End of inner try"
        IL_010b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_0110:  ldarg.0
        IL_0111:  ldc.i4.4
        IL_0112:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0117:  ldc.i4.1
        IL_0118:  stloc.0
        IL_0119:  leave      IL_01be

        IL_011e:  ldarg.0
        IL_011f:  ldc.i4.s   -4
        IL_0121:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0126:  ldstr      "End of inner try - 2"
        IL_012b:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0130:  ldarg.0
        IL_0131:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'()
        IL_0136:  br.s       IL_0140

        IL_0138:  ldarg.0
        IL_0139:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'()
        IL_013e:  br.s       IL_017a

        IL_0140:  ldstr      "End of outer try - 1"
        IL_0145:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_014a:  ldarg.0
        IL_014b:  ldstr      "End of outer try"
        IL_0150:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_0155:  ldarg.0
        IL_0156:  ldc.i4.5
        IL_0157:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_015c:  ldc.i4.1
        IL_015d:  stloc.0
        IL_015e:  leave.s    IL_01be

        IL_0160:  ldarg.0
        IL_0161:  ldc.i4.s   -3
        IL_0163:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0168:  ldstr      "End of outer try - 2"
        IL_016d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0172:  ldarg.0
        IL_0173:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'()
        IL_0178:  br.s       IL_0182

        IL_017a:  ldarg.0
        IL_017b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'()
        IL_0180:  leave.s    IL_01be

        IL_0182:  ldstr      "End of method - 1"
        IL_0187:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_018c:  ldarg.0
        IL_018d:  ldstr      "End of method"
        IL_0192:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_0197:  ldarg.0
        IL_0198:  ldc.i4.6
        IL_0199:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_019e:  ldc.i4.1
        IL_019f:  stloc.0
        IL_01a0:  leave.s    IL_01be

        IL_01a2:  ldarg.0
        IL_01a3:  ldc.i4.m1
        IL_01a4:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_01a9:  ldstr      "End of method - 2"
        IL_01ae:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01b3:  ldc.i4.0
        IL_01b4:  stloc.0
        IL_01b5:  leave.s    IL_01be

      }  // end .try
      fault
      {
        IL_01b7:  ldarg.0
        IL_01b8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::System.IDisposable.Dispose()
        IL_01bd:  endfinally
      }  // end handler
      IL_01be:  ldloc.0
      IL_01bf:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0007:  ldstr      "Outer Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'

    .method private hidebysig instance void 
            '<>m__Finally2'() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   -3
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0008:  ldstr      "Inner Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>3__breakInMiddle'
      IL_0030:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::breakInMiddle
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::System.Collections.IEnumerable.GetEnumerator

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<YieldReturnWithNestedTryFinally>d__10'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithNestedTryFinally>d__10'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithNestedTryFinally>d__10'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'
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
    .field private class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<>3__input'
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap1'
    .field private string '<line>5__3'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       89 (0x59)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -5
      IL_000a:  sub
      IL_000b:  switch     ( 
                            IL_004e,
                            IL_0031,
                            IL_0031,
                            IL_0058,
                            IL_0058,
                            IL_0058,
                            IL_0031)
      IL_002c:  ldloc.0
      IL_002d:  ldc.i4.8
      IL_002e:  beq.s      IL_004e

      IL_0030:  ret

      IL_0031:  nop
      .try
      {
        IL_0032:  ldloc.0
        IL_0033:  ldc.i4.s   -4
        IL_0035:  beq.s      IL_003d

        IL_0037:  ldloc.0
        IL_0038:  ldc.i4.1
        IL_0039:  beq.s      IL_003d

        IL_003b:  leave.s    IL_0058

        IL_003d:  nop
        .try
        {
          IL_003e:  leave.s    IL_0058

        }  // end .try
        finally
        {
          IL_0040:  ldarg.0
          IL_0041:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally2'()
          IL_0046:  endfinally
        }  // end handler
      }  // end .try
      finally
      {
        IL_0047:  ldarg.0
        IL_0048:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally1'()
        IL_004d:  endfinally
      }  // end handler
      IL_004e:  nop
      .try
      {
        IL_004f:  leave.s    IL_0058

      }  // end .try
      finally
      {
        IL_0051:  ldarg.0
        IL_0052:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally3'()
        IL_0057:  endfinally
      }  // end handler
      IL_0058:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       486 (0x1e6)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1,
               string V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0038,
                              IL_008d,
                              IL_00d5,
                              IL_00f5,
                              IL_0115,
                              IL_0135,
                              IL_0155,
                              IL_0172,
                              IL_01b7)
        IL_0031:  ldc.i4.0
        IL_0032:  stloc.0
        IL_0033:  leave      IL_01e4

        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.m1
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_003f:  ldarg.0
        IL_0040:  ldarg.0
        IL_0041:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::input
        IL_0046:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_004b:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_0050:  ldarg.0
        IL_0051:  ldc.i4.s   -3
        IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0058:  br.s       IL_00a2

        IL_005a:  ldarg.0
        IL_005b:  ldarg.0
        IL_005c:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_0061:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_0066:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__3'
        IL_006b:  ldarg.0
        IL_006c:  ldc.i4.s   -4
        IL_006e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0073:  ldarg.0
        IL_0074:  ldarg.0
        IL_0075:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__3'
        IL_007a:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_007f:  ldarg.0
        IL_0080:  ldc.i4.1
        IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0086:  ldc.i4.1
        IL_0087:  stloc.0
        IL_0088:  leave      IL_01e4

        IL_008d:  ldarg.0
        IL_008e:  ldc.i4.s   -4
        IL_0090:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0095:  ldarg.0
        IL_0096:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally2'()
        IL_009b:  ldarg.0
        IL_009c:  ldnull
        IL_009d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__3'
        IL_00a2:  ldarg.0
        IL_00a3:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_00a8:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00ad:  brtrue.s   IL_005a

        IL_00af:  ldarg.0
        IL_00b0:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally1'()
        IL_00b5:  ldarg.0
        IL_00b6:  ldnull
        IL_00b7:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_00bc:  ldarg.0
        IL_00bd:  ldstr      "A"
        IL_00c2:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_00c7:  ldarg.0
        IL_00c8:  ldc.i4.2
        IL_00c9:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00ce:  ldc.i4.1
        IL_00cf:  stloc.0
        IL_00d0:  leave      IL_01e4

        IL_00d5:  ldarg.0
        IL_00d6:  ldc.i4.m1
        IL_00d7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00dc:  ldarg.0
        IL_00dd:  ldstr      "B"
        IL_00e2:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_00e7:  ldarg.0
        IL_00e8:  ldc.i4.3
        IL_00e9:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00ee:  ldc.i4.1
        IL_00ef:  stloc.0
        IL_00f0:  leave      IL_01e4

        IL_00f5:  ldarg.0
        IL_00f6:  ldc.i4.m1
        IL_00f7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00fc:  ldarg.0
        IL_00fd:  ldstr      "C"
        IL_0102:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_0107:  ldarg.0
        IL_0108:  ldc.i4.4
        IL_0109:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_010e:  ldc.i4.1
        IL_010f:  stloc.0
        IL_0110:  leave      IL_01e4

        IL_0115:  ldarg.0
        IL_0116:  ldc.i4.m1
        IL_0117:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_011c:  ldarg.0
        IL_011d:  ldstr      "D"
        IL_0122:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_0127:  ldarg.0
        IL_0128:  ldc.i4.5
        IL_0129:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_012e:  ldc.i4.1
        IL_012f:  stloc.0
        IL_0130:  leave      IL_01e4

        IL_0135:  ldarg.0
        IL_0136:  ldc.i4.m1
        IL_0137:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_013c:  ldarg.0
        IL_013d:  ldstr      "E"
        IL_0142:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_0147:  ldarg.0
        IL_0148:  ldc.i4.6
        IL_0149:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_014e:  ldc.i4.1
        IL_014f:  stloc.0
        IL_0150:  leave      IL_01e4

        IL_0155:  ldarg.0
        IL_0156:  ldc.i4.m1
        IL_0157:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_015c:  ldarg.0
        IL_015d:  ldstr      "F"
        IL_0162:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_0167:  ldarg.0
        IL_0168:  ldc.i4.7
        IL_0169:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_016e:  ldc.i4.1
        IL_016f:  stloc.0
        IL_0170:  leave.s    IL_01e4

        IL_0172:  ldarg.0
        IL_0173:  ldc.i4.m1
        IL_0174:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0179:  ldarg.0
        IL_017a:  ldarg.0
        IL_017b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::input
        IL_0180:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0185:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_018a:  ldarg.0
        IL_018b:  ldc.i4.s   -5
        IL_018d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0192:  br.s       IL_01bf

        IL_0194:  ldarg.0
        IL_0195:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_019a:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_019f:  stloc.2
        IL_01a0:  ldarg.0
        IL_01a1:  ldloc.2
        IL_01a2:  callvirt   instance string [mscorlib]System.String::ToUpper()
        IL_01a7:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_01ac:  ldarg.0
        IL_01ad:  ldc.i4.8
        IL_01ae:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_01b3:  ldc.i4.1
        IL_01b4:  stloc.0
        IL_01b5:  leave.s    IL_01e4

        IL_01b7:  ldarg.0
        IL_01b8:  ldc.i4.s   -5
        IL_01ba:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_01bf:  ldarg.0
        IL_01c0:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_01c5:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_01ca:  brtrue.s   IL_0194

        IL_01cc:  ldarg.0
        IL_01cd:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally3'()
        IL_01d2:  ldarg.0
        IL_01d3:  ldnull
        IL_01d4:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
        IL_01d9:  ldc.i4.0
        IL_01da:  stloc.0
        IL_01db:  leave.s    IL_01e4

      }  // end .try
      fault
      {
        IL_01dd:  ldarg.0
        IL_01de:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.IDisposable.Dispose()
        IL_01e3:  endfinally
      }  // end handler
      IL_01e4:  ldloc.0
      IL_01e5:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally1'

    .method private hidebysig instance void 
            '<>m__Finally2'() cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   -3
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0008:  ldstr      "Processed "
      IL_000d:  ldarg.0
      IL_000e:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__3'
      IL_0013:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001d:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally2'

    .method private hidebysig instance void 
            '<>m__Finally3'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>7__wrap1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally3'

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
            'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>3__input'
      IL_0030:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::input
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.Collections.IEnumerable.GetEnumerator

    .property instance string 'System.Collections.Generic.IEnumerator<System.String>.Current'()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'System.Collections.Generic.IEnumerator<System.String>.get_Current'()
    } // end of property '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'System.Collections.Generic.IEnumerator<System.String>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass12_0'
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
    } // end of method '<>c__DisplayClass12_0'::.ctor

    .method assembly hidebysig instance string 
            '<YieldReturnWithAnonymousMethods1>b__0'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::line
      IL_0006:  ret
    } // end of method '<>c__DisplayClass12_0'::'<YieldReturnWithAnonymousMethods1>b__0'

  } // end of class '<>c__DisplayClass12_0'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithAnonymousMethods1>d__12'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private class [mscorlib]System.Func`1<string> '<>2__current'
    .field private int32 '<>l__initialThreadId'
    .field private class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<>3__input'
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap1'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0010

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  bne.un.s   IL_001a

      IL_0010:  nop
      .try
      {
        IL_0011:  leave.s    IL_001a

      }  // end .try
      finally
      {
        IL_0013:  ldarg.0
        IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>m__Finally1'()
        IL_0019:  endfinally
      }  // end handler
      IL_001a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       154 (0x9a)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0' V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0015

        IL_000a:  ldloc.1
        IL_000b:  ldc.i4.1
        IL_000c:  beq.s      IL_006b

        IL_000e:  ldc.i4.0
        IL_000f:  stloc.0
        IL_0010:  leave      IL_0098

        IL_0015:  ldarg.0
        IL_0016:  ldc.i4.m1
        IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_001c:  ldarg.0
        IL_001d:  ldarg.0
        IL_001e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::input
        IL_0023:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0028:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>7__wrap1'
        IL_002d:  ldarg.0
        IL_002e:  ldc.i4.s   -3
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0035:  br.s       IL_0073

        IL_0037:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::.ctor()
        IL_003c:  stloc.2
        IL_003d:  ldloc.2
        IL_003e:  ldarg.0
        IL_003f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>7__wrap1'
        IL_0044:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_0049:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::line
        IL_004e:  ldarg.0
        IL_004f:  ldloc.2
        IL_0050:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::'<YieldReturnWithAnonymousMethods1>b__0'()
        IL_0056:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_005b:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>2__current'
        IL_0060:  ldarg.0
        IL_0061:  ldc.i4.1
        IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0067:  ldc.i4.1
        IL_0068:  stloc.0
        IL_0069:  leave.s    IL_0098

        IL_006b:  ldarg.0
        IL_006c:  ldc.i4.s   -3
        IL_006e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0073:  ldarg.0
        IL_0074:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>7__wrap1'
        IL_0079:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_007e:  brtrue.s   IL_0037

        IL_0080:  ldarg.0
        IL_0081:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>m__Finally1'()
        IL_0086:  ldarg.0
        IL_0087:  ldnull
        IL_0088:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>7__wrap1'
        IL_008d:  ldc.i4.0
        IL_008e:  stloc.0
        IL_008f:  leave.s    IL_0098

      }  // end .try
      fault
      {
        IL_0091:  ldarg.0
        IL_0092:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::System.IDisposable.Dispose()
        IL_0097:  endfinally
      }  // end handler
      IL_0098:  ldloc.0
      IL_0099:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>7__wrap1'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>7__wrap1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance class [mscorlib]System.Func`1<string> 
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> 
            'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>3__input'
      IL_0030:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::input
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::System.Collections.IEnumerable.GetEnumerator

    .property instance class [mscorlib]System.Func`1<string>
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'()
    {
      .get instance class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'()
    } // end of property '<YieldReturnWithAnonymousMethods1>d__12'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithAnonymousMethods1>d__12'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithAnonymousMethods1>d__12'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass13_0'
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
    } // end of method '<>c__DisplayClass13_0'::.ctor

    .method assembly hidebysig instance string 
            '<YieldReturnWithAnonymousMethods2>b__0'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::copy
      IL_0006:  ret
    } // end of method '<>c__DisplayClass13_0'::'<YieldReturnWithAnonymousMethods2>b__0'

  } // end of class '<>c__DisplayClass13_0'

  .class auto ansi sealed nested private beforefieldinit '<YieldReturnWithAnonymousMethods2>d__13'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private class [mscorlib]System.Func`1<string> '<>2__current'
    .field private int32 '<>l__initialThreadId'
    .field private class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<string> '<>3__input'
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>7__wrap1'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0010

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  bne.un.s   IL_001a

      IL_0010:  nop
      .try
      {
        IL_0011:  leave.s    IL_001a

      }  // end .try
      finally
      {
        IL_0013:  ldarg.0
        IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>m__Finally1'()
        IL_0019:  endfinally
      }  // end handler
      IL_001a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       156 (0x9c)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1,
               string V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0' V_3)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0015

        IL_000a:  ldloc.1
        IL_000b:  ldc.i4.1
        IL_000c:  beq.s      IL_006d

        IL_000e:  ldc.i4.0
        IL_000f:  stloc.0
        IL_0010:  leave      IL_009a

        IL_0015:  ldarg.0
        IL_0016:  ldc.i4.m1
        IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_001c:  ldarg.0
        IL_001d:  ldarg.0
        IL_001e:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::input
        IL_0023:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0028:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>7__wrap1'
        IL_002d:  ldarg.0
        IL_002e:  ldc.i4.s   -3
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0035:  br.s       IL_0075

        IL_0037:  ldarg.0
        IL_0038:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>7__wrap1'
        IL_003d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_0042:  stloc.2
        IL_0043:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::.ctor()
        IL_0048:  stloc.3
        IL_0049:  ldloc.3
        IL_004a:  ldloc.2
        IL_004b:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::copy
        IL_0050:  ldarg.0
        IL_0051:  ldloc.3
        IL_0052:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::'<YieldReturnWithAnonymousMethods2>b__0'()
        IL_0058:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_005d:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>2__current'
        IL_0062:  ldarg.0
        IL_0063:  ldc.i4.1
        IL_0064:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0069:  ldc.i4.1
        IL_006a:  stloc.0
        IL_006b:  leave.s    IL_009a

        IL_006d:  ldarg.0
        IL_006e:  ldc.i4.s   -3
        IL_0070:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0075:  ldarg.0
        IL_0076:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>7__wrap1'
        IL_007b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0080:  brtrue.s   IL_0037

        IL_0082:  ldarg.0
        IL_0083:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>m__Finally1'()
        IL_0088:  ldarg.0
        IL_0089:  ldnull
        IL_008a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>7__wrap1'
        IL_008f:  ldc.i4.0
        IL_0090:  stloc.0
        IL_0091:  leave.s    IL_009a

      }  // end .try
      fault
      {
        IL_0093:  ldarg.0
        IL_0094:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::System.IDisposable.Dispose()
        IL_0099:  endfinally
      }  // end handler
      IL_009a:  ldloc.0
      IL_009b:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>7__wrap1'
      IL_000d:  brfalse.s  IL_001a

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>7__wrap1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance class [mscorlib]System.Func`1<string> 
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> 
            'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>3__input'
      IL_0030:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::input
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::System.Collections.IEnumerable.GetEnumerator

    .property instance class [mscorlib]System.Func`1<string>
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'()
    {
      .get instance class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'()
    } // end of property '<YieldReturnWithAnonymousMethods2>d__13'::'System.Collections.Generic.IEnumerator<System.Func<System.String>>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldReturnWithAnonymousMethods2>d__13'::System.Collections.IEnumerator.Current
  } // end of class '<YieldReturnWithAnonymousMethods2>d__13'

  .class auto ansi sealed nested private beforefieldinit '<GetEvenNumbers>d__14'
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
    .field private int32 n
    .field public int32 '<>3__n'
    .field private int32 '<i>5__2'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<GetEvenNumbers>d__14'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<GetEvenNumbers>d__14'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       102 (0x66)
      .maxstack  3
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0010

      IL_000a:  ldloc.0
      IL_000b:  ldc.i4.1
      IL_000c:  beq.s      IL_003f

      IL_000e:  ldc.i4.0
      IL_000f:  ret

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.m1
      IL_0012:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__2'
      IL_001e:  br.s       IL_0056

      IL_0020:  ldarg.0
      IL_0021:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__2'
      IL_0026:  ldc.i4.2
      IL_0027:  rem
      IL_0028:  brtrue.s   IL_0046

      IL_002a:  ldarg.0
      IL_002b:  ldarg.0
      IL_002c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__2'
      IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>2__current'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_003d:  ldc.i4.1
      IL_003e:  ret

      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.m1
      IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_0046:  ldarg.0
      IL_0047:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__2'
      IL_004c:  stloc.1
      IL_004d:  ldarg.0
      IL_004e:  ldloc.1
      IL_004f:  ldc.i4.1
      IL_0050:  add
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__2'
      IL_0056:  ldarg.0
      IL_0057:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__2'
      IL_005c:  ldarg.0
      IL_005d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::n
      IL_0062:  blt.s      IL_0020

      IL_0064:  ldc.i4.0
      IL_0065:  ret
    } // end of method '<GetEvenNumbers>d__14'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>2__current'
      IL_0006:  ret
    } // end of method '<GetEvenNumbers>d__14'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<GetEvenNumbers>d__14'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<GetEvenNumbers>d__14'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>3__n'
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::n
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<GetEvenNumbers>d__14'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<GetEvenNumbers>d__14'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<GetEvenNumbers>d__14'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::System.Collections.IEnumerator.get_Current()
    } // end of property '<GetEvenNumbers>d__14'::System.Collections.IEnumerator.Current
  } // end of class '<GetEvenNumbers>d__14'

  .class auto ansi sealed nested private beforefieldinit '<ExceptionHandling>d__15'
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<char>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<char>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private char '<>2__current'
    .field private int32 '<>l__initialThreadId'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<ExceptionHandling>d__15'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0010

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.3
      IL_000e:  bne.un.s   IL_001a

      IL_0010:  nop
      .try
      {
        IL_0011:  leave.s    IL_001a

      }  // end .try
      finally
      {
        IL_0013:  ldarg.0
        IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>m__Finally1'()
        IL_0019:  endfinally
      }  // end handler
      IL_001a:  ret
    } // end of method '<ExceptionHandling>d__15'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       200 (0xc8)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0024,
                              IL_0041,
                              IL_0074,
                              IL_00ad)
        IL_001d:  ldc.i4.0
        IL_001e:  stloc.0
        IL_001f:  leave      IL_00c6

        IL_0024:  ldarg.0
        IL_0025:  ldc.i4.m1
        IL_0026:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.s   97
        IL_002e:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
        IL_0033:  ldarg.0
        IL_0034:  ldc.i4.1
        IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_003a:  ldc.i4.1
        IL_003b:  stloc.0
        IL_003c:  leave      IL_00c6

        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.m1
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        .try
        {
          IL_0048:  ldstr      "1 - try"
          IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0052:  leave.s    IL_0061

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_0054:  pop
          IL_0055:  ldstr      "1 - catch"
          IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005f:  leave.s    IL_0061

        }  // end handler
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.s   98
        IL_0064:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.2
        IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_0070:  ldc.i4.1
        IL_0071:  stloc.0
        IL_0072:  leave.s    IL_00c6

        IL_0074:  ldarg.0
        IL_0075:  ldc.i4.m1
        IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.s   -3
        IL_007e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        .try
        {
          IL_0083:  ldstr      "2 - try"
          IL_0088:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_008d:  leave.s    IL_009a

        }  // end .try
        finally
        {
          IL_008f:  ldstr      "2 - finally"
          IL_0094:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0099:  endfinally
        }  // end handler
        IL_009a:  ldarg.0
        IL_009b:  ldc.i4.s   99
        IL_009d:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
        IL_00a2:  ldarg.0
        IL_00a3:  ldc.i4.3
        IL_00a4:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_00a9:  ldc.i4.1
        IL_00aa:  stloc.0
        IL_00ab:  leave.s    IL_00c6

        IL_00ad:  ldarg.0
        IL_00ae:  ldc.i4.s   -3
        IL_00b0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_00b5:  ldarg.0
        IL_00b6:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>m__Finally1'()
        IL_00bb:  ldc.i4.0
        IL_00bc:  stloc.0
        IL_00bd:  leave.s    IL_00c6

      }  // end .try
      fault
      {
        IL_00bf:  ldarg.0
        IL_00c0:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::System.IDisposable.Dispose()
        IL_00c5:  endfinally
      }  // end handler
      IL_00c6:  ldloc.0
      IL_00c7:  ret
    } // end of method '<ExceptionHandling>d__15'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_0007:  ldstr      "outer finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<ExceptionHandling>d__15'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance char  'System.Collections.Generic.IEnumerator<System.Char>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<char>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
      IL_0006:  ret
    } // end of method '<ExceptionHandling>d__15'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<ExceptionHandling>d__15'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Char
      IL_000b:  ret
    } // end of method '<ExceptionHandling>d__15'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> 
            'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<char>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<ExceptionHandling>d__15'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<ExceptionHandling>d__15'::System.Collections.IEnumerable.GetEnumerator

    .property instance char 'System.Collections.Generic.IEnumerator<System.Char>.Current'()
    {
      .get instance char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'System.Collections.Generic.IEnumerator<System.Char>.get_Current'()
    } // end of property '<ExceptionHandling>d__15'::'System.Collections.Generic.IEnumerator<System.Char>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::System.Collections.IEnumerator.get_Current()
    } // end of property '<ExceptionHandling>d__15'::System.Collections.IEnumerator.Current
  } // end of class '<ExceptionHandling>d__15'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInCatch>d__16'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInCatch>d__16'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldBreakInCatch>d__16'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       101 (0x65)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_0032,
                            IL_005a)
      IL_0019:  ldc.i4.0
      IL_001a:  ret

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.m1
      IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.0
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>2__current'
      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0030:  ldc.i4.1
      IL_0031:  ret

      IL_0032:  ldarg.0
      IL_0033:  ldc.i4.m1
      IL_0034:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      .try
      {
        IL_0039:  ldstr      "In Try"
        IL_003e:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0043:  leave.s    IL_004a

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0045:  pop
        IL_0046:  ldc.i4.0
        IL_0047:  stloc.0
        IL_0048:  leave.s    IL_0063

      }  // end handler
      IL_004a:  ldarg.0
      IL_004b:  ldc.i4.1
      IL_004c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>2__current'
      IL_0051:  ldarg.0
      IL_0052:  ldc.i4.2
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0058:  ldc.i4.1
      IL_0059:  ret

      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.m1
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0061:  ldc.i4.0
      IL_0062:  ret

      IL_0063:  ldloc.0
      IL_0064:  ret
    } // end of method '<YieldBreakInCatch>d__16'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInCatch>d__16'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInCatch>d__16'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInCatch>d__16'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakInCatch>d__16'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInCatch>d__16'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInCatch>d__16'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInCatch>d__16'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInCatch>d__16'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInCatchInTryFinally>d__17'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       29 (0x1d)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0012

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  sub
      IL_000f:  ldc.i4.1
      IL_0010:  bgt.un.s   IL_001c

      IL_0012:  nop
      .try
      {
        IL_0013:  leave.s    IL_001c

      }  // end .try
      finally
      {
        IL_0015:  ldarg.0
        IL_0016:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'()
        IL_001b:  endfinally
      }  // end handler
      IL_001c:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       142 (0x8e)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001d,
                              IL_003e,
                              IL_0069)
        IL_0019:  ldc.i4.0
        IL_001a:  stloc.0
        IL_001b:  leave.s    IL_008c

        IL_001d:  ldarg.0
        IL_001e:  ldc.i4.m1
        IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0024:  ldarg.0
        IL_0025:  ldc.i4.s   -3
        IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_002c:  ldarg.0
        IL_002d:  ldc.i4.0
        IL_002e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>2__current'
        IL_0033:  ldarg.0
        IL_0034:  ldc.i4.1
        IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_003a:  ldc.i4.1
        IL_003b:  stloc.0
        IL_003c:  leave.s    IL_008c

        IL_003e:  ldarg.0
        IL_003f:  ldc.i4.s   -3
        IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        .try
        {
          IL_0046:  ldstr      "In Try"
          IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0050:  leave.s    IL_0057

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0052:  pop
          IL_0053:  ldc.i4.0
          IL_0054:  stloc.0
          IL_0055:  leave.s    IL_0079

        }  // end handler
        IL_0057:  ldarg.0
        IL_0058:  ldc.i4.1
        IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>2__current'
        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.2
        IL_0060:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0065:  ldc.i4.1
        IL_0066:  stloc.0
        IL_0067:  leave.s    IL_008c

        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.s   -3
        IL_006c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0071:  ldarg.0
        IL_0072:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'()
        IL_0077:  br.s       IL_0081

        IL_0079:  ldarg.0
        IL_007a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'()
        IL_007f:  leave.s    IL_008c

        IL_0081:  ldc.i4.0
        IL_0082:  stloc.0
        IL_0083:  leave.s    IL_008c

      }  // end .try
      fault
      {
        IL_0085:  ldarg.0
        IL_0086:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::System.IDisposable.Dispose()
        IL_008b:  endfinally
      }  // end handler
      IL_008c:  ldloc.0
      IL_008d:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInCatchInTryFinally>d__17'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInCatchInTryFinally>d__17'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInCatchInTryFinally>d__17'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInTryCatchInTryFinally>d__18'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       29 (0x1d)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0012

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  sub
      IL_000f:  ldc.i4.1
      IL_0010:  bgt.un.s   IL_001c

      IL_0012:  nop
      .try
      {
        IL_0013:  leave.s    IL_001c

      }  // end .try
      finally
      {
        IL_0015:  ldarg.0
        IL_0016:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'()
        IL_001b:  endfinally
      }  // end handler
      IL_001c:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       152 (0x98)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001d,
                              IL_003e,
                              IL_0073)
        IL_0019:  ldc.i4.0
        IL_001a:  stloc.0
        IL_001b:  leave.s    IL_0096

        IL_001d:  ldarg.0
        IL_001e:  ldc.i4.m1
        IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_0024:  ldarg.0
        IL_0025:  ldc.i4.s   -3
        IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_002c:  ldarg.0
        IL_002d:  ldc.i4.0
        IL_002e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>2__current'
        IL_0033:  ldarg.0
        IL_0034:  ldc.i4.1
        IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_003a:  ldc.i4.1
        IL_003b:  stloc.0
        IL_003c:  leave.s    IL_0096

        IL_003e:  ldarg.0
        IL_003f:  ldc.i4.s   -3
        IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        .try
        {
          IL_0046:  ldstr      "In Try"
          IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0050:  ldc.i4.0
          IL_0051:  stloc.0
          IL_0052:  leave.s    IL_0083

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0054:  pop
          IL_0055:  ldstr      "Catch"
          IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005f:  leave.s    IL_0061

        }  // end handler
        IL_0061:  ldarg.0
        IL_0062:  ldc.i4.1
        IL_0063:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>2__current'
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.2
        IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_006f:  ldc.i4.1
        IL_0070:  stloc.0
        IL_0071:  leave.s    IL_0096

        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.s   -3
        IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_007b:  ldarg.0
        IL_007c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'()
        IL_0081:  br.s       IL_008b

        IL_0083:  ldarg.0
        IL_0084:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'()
        IL_0089:  leave.s    IL_0096

        IL_008b:  ldc.i4.0
        IL_008c:  stloc.0
        IL_008d:  leave.s    IL_0096

      }  // end .try
      fault
      {
        IL_008f:  ldarg.0
        IL_0090:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::System.IDisposable.Dispose()
        IL_0095:  endfinally
      }  // end handler
      IL_0096:  ldloc.0
      IL_0097:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInTryCatchInTryFinally>d__18'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInTryCatchInTryFinally>d__18'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInTryCatchInTryFinally>d__18'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakInTryFinallyInTryFinally>d__19'
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
    .field private bool b
    .field public bool '<>3__b'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       29 (0x1d)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0012

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  sub
      IL_000f:  ldc.i4.1
      IL_0010:  bgt.un.s   IL_001c

      IL_0012:  nop
      .try
      {
        IL_0013:  leave.s    IL_001c

      }  // end .try
      finally
      {
        IL_0015:  ldarg.0
        IL_0016:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'()
        IL_001b:  endfinally
      }  // end handler
      IL_001c:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       163 (0xa3)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0020,
                              IL_0041,
                              IL_007e)
        IL_0019:  ldc.i4.0
        IL_001a:  stloc.0
        IL_001b:  leave      IL_00a1

        IL_0020:  ldarg.0
        IL_0021:  ldc.i4.m1
        IL_0022:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0027:  ldarg.0
        IL_0028:  ldc.i4.s   -3
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_002f:  ldarg.0
        IL_0030:  ldc.i4.0
        IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>2__current'
        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.1
        IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_003d:  ldc.i4.1
        IL_003e:  stloc.0
        IL_003f:  leave.s    IL_00a1

        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.s   -3
        IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        .try
        {
          IL_0049:  ldstr      "In Try"
          IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0053:  ldarg.0
          IL_0054:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::b
          IL_0059:  brfalse.s  IL_005f

          IL_005b:  ldc.i4.0
          IL_005c:  stloc.0
          IL_005d:  leave.s    IL_008e

          IL_005f:  leave.s    IL_006c

        }  // end .try
        finally
        {
          IL_0061:  ldstr      "Inner Finally"
          IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_006b:  endfinally
        }  // end handler
        IL_006c:  ldarg.0
        IL_006d:  ldc.i4.1
        IL_006e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>2__current'
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.2
        IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_007a:  ldc.i4.1
        IL_007b:  stloc.0
        IL_007c:  leave.s    IL_00a1

        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.s   -3
        IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0086:  ldarg.0
        IL_0087:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'()
        IL_008c:  br.s       IL_0096

        IL_008e:  ldarg.0
        IL_008f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'()
        IL_0094:  leave.s    IL_00a1

        IL_0096:  ldc.i4.0
        IL_0097:  stloc.0
        IL_0098:  leave.s    IL_00a1

      }  // end .try
      fault
      {
        IL_009a:  ldarg.0
        IL_009b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::System.IDisposable.Dispose()
        IL_00a0:  endfinally
      }  // end handler
      IL_00a1:  ldloc.0
      IL_00a2:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>3__b'
      IL_0030:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::b
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakInTryFinallyInTryFinally>d__19'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakInTryFinallyInTryFinally>d__19'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakInTryFinallyInTryFinally>d__19'

  .class auto ansi sealed nested private beforefieldinit '<YieldBreakOnly>d__20'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<YieldBreakOnly>d__20'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<YieldBreakOnly>d__20'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       21 (0x15)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  ldc.i4.0
      IL_000b:  ret

      IL_000c:  ldarg.0
      IL_000d:  ldc.i4.m1
      IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_0013:  ldc.i4.0
      IL_0014:  ret
    } // end of method '<YieldBreakOnly>d__20'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>2__current'
      IL_0006:  ret
    } // end of method '<YieldBreakOnly>d__20'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<YieldBreakOnly>d__20'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<YieldBreakOnly>d__20'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<YieldBreakOnly>d__20'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<YieldBreakOnly>d__20'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<YieldBreakOnly>d__20'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::System.Collections.IEnumerator.get_Current()
    } // end of property '<YieldBreakOnly>d__20'::System.Collections.IEnumerator.Current
  } // end of class '<YieldBreakOnly>d__20'

  .class auto ansi sealed nested private beforefieldinit '<UnconditionalThrowInTryFinally>d__21'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0010

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  bne.un.s   IL_001a

      IL_0010:  nop
      .try
      {
        IL_0011:  leave.s    IL_001a

      }  // end .try
      finally
      {
        IL_0013:  ldarg.0
        IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>m__Finally1'()
        IL_0019:  endfinally
      }  // end handler
      IL_001a:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       74 (0x4a)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldloc.1
        IL_000b:  ldc.i4.1
        IL_000c:  beq.s      IL_0033

        IL_000e:  ldc.i4.0
        IL_000f:  stloc.0
        IL_0010:  leave.s    IL_0048

        IL_0012:  ldarg.0
        IL_0013:  ldc.i4.m1
        IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0019:  ldarg.0
        IL_001a:  ldc.i4.s   -3
        IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.0
        IL_0023:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>2__current'
        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.1
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_002f:  ldc.i4.1
        IL_0030:  stloc.0
        IL_0031:  leave.s    IL_0048

        IL_0033:  ldarg.0
        IL_0034:  ldc.i4.s   -3
        IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_003b:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
        IL_0040:  throw

      }  // end .try
      fault
      {
        IL_0041:  ldarg.0
        IL_0042:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::System.IDisposable.Dispose()
        IL_0047:  endfinally
      }  // end handler
      IL_0048:  ldloc.0
      IL_0049:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_0007:  ldstr      "Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>2__current'
      IL_0006:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<UnconditionalThrowInTryFinally>d__21'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::System.Collections.IEnumerator.get_Current()
    } // end of property '<UnconditionalThrowInTryFinally>d__21'::System.Collections.IEnumerator.Current
  } // end of class '<UnconditionalThrowInTryFinally>d__21'

  .class auto ansi sealed nested private beforefieldinit '<NestedTryFinallyStartingOnSamePosition>d__22'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       48 (0x30)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -4
      IL_000a:  sub
      IL_000b:  ldc.i4.1
      IL_000c:  ble.un.s   IL_0012

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  bne.un.s   IL_002f

      IL_0012:  nop
      .try
      {
        IL_0013:  ldloc.0
        IL_0014:  ldc.i4.s   -4
        IL_0016:  beq.s      IL_001e

        IL_0018:  ldloc.0
        IL_0019:  ldc.i4.1
        IL_001a:  beq.s      IL_001e

        IL_001c:  leave.s    IL_002f

        IL_001e:  nop
        .try
        {
          IL_001f:  leave.s    IL_002f

        }  // end .try
        finally
        {
          IL_0021:  ldarg.0
          IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally2'()
          IL_0027:  endfinally
        }  // end handler
      }  // end .try
      finally
      {
        IL_0028:  ldarg.0
        IL_0029:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally1'()
        IL_002e:  endfinally
      }  // end handler
      IL_002f:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       92 (0x5c)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  ldloc.1
        IL_000b:  ldc.i4.1
        IL_000c:  beq.s      IL_003b

        IL_000e:  ldc.i4.0
        IL_000f:  stloc.0
        IL_0010:  leave.s    IL_005a

        IL_0012:  ldarg.0
        IL_0013:  ldc.i4.m1
        IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0019:  ldarg.0
        IL_001a:  ldc.i4.s   -3
        IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.s   -4
        IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0029:  ldarg.0
        IL_002a:  ldc.i4.0
        IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>2__current'
        IL_0030:  ldarg.0
        IL_0031:  ldc.i4.1
        IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0037:  ldc.i4.1
        IL_0038:  stloc.0
        IL_0039:  leave.s    IL_005a

        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.s   -4
        IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0043:  ldarg.0
        IL_0044:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally2'()
        IL_0049:  ldarg.0
        IL_004a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally1'()
        IL_004f:  ldc.i4.0
        IL_0050:  stloc.0
        IL_0051:  leave.s    IL_005a

      }  // end .try
      fault
      {
        IL_0053:  ldarg.0
        IL_0054:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::System.IDisposable.Dispose()
        IL_0059:  endfinally
      }  // end handler
      IL_005a:  ldloc.0
      IL_005b:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0007:  ldstr      "Outer Finally"
      IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0011:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally1'

    .method private hidebysig instance void 
            '<>m__Finally2'() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   -3
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0008:  ldstr      "Inner Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally2'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>2__current'
      IL_0006:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<NestedTryFinallyStartingOnSamePosition>d__22'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::System.Collections.IEnumerator.get_Current()
    } // end of property '<NestedTryFinallyStartingOnSamePosition>d__22'::System.Collections.IEnumerator.Current
  } // end of class '<NestedTryFinallyStartingOnSamePosition>d__22'

  .class auto ansi sealed nested private beforefieldinit '<LocalInFinally>d__23`1'<([mscorlib]System.IDisposable) T>
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
    .field private !T a
    .field public !T '<>3__a'
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
      IL_0008:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<LocalInFinally>d__23`1'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       27 (0x1b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0010

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.2
      IL_000e:  bne.un.s   IL_001a

      IL_0010:  nop
      .try
      {
        IL_0011:  leave.s    IL_001a

      }  // end .try
      finally
      {
        IL_0013:  ldarg.0
        IL_0014:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>m__Finally1'()
        IL_0019:  endfinally
      }  // end handler
      IL_001a:  ret
    } // end of method '<LocalInFinally>d__23`1'::System.IDisposable.Dispose

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
        IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0021,
                              IL_003a,
                              IL_005b,
                              IL_007b)
        IL_001d:  ldc.i4.0
        IL_001e:  stloc.0
        IL_001f:  leave.s    IL_008d

        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.m1
        IL_0023:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.1
        IL_002a:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
        IL_002f:  ldarg.0
        IL_0030:  ldc.i4.1
        IL_0031:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0036:  ldc.i4.1
        IL_0037:  stloc.0
        IL_0038:  leave.s    IL_008d

        IL_003a:  ldarg.0
        IL_003b:  ldc.i4.m1
        IL_003c:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.s   -3
        IL_0044:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0049:  ldarg.0
        IL_004a:  ldc.i4.2
        IL_004b:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
        IL_0050:  ldarg.0
        IL_0051:  ldc.i4.2
        IL_0052:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0057:  ldc.i4.1
        IL_0058:  stloc.0
        IL_0059:  leave.s    IL_008d

        IL_005b:  ldarg.0
        IL_005c:  ldc.i4.s   -3
        IL_005e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0063:  ldarg.0
        IL_0064:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>m__Finally1'()
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.3
        IL_006b:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
        IL_0070:  ldarg.0
        IL_0071:  ldc.i4.3
        IL_0072:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0077:  ldc.i4.1
        IL_0078:  stloc.0
        IL_0079:  leave.s    IL_008d

        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.m1
        IL_007d:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0082:  ldc.i4.0
        IL_0083:  stloc.0
        IL_0084:  leave.s    IL_008d

      }  // end .try
      fault
      {
        IL_0086:  ldarg.0
        IL_0087:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::System.IDisposable.Dispose()
        IL_008c:  endfinally
      }  // end handler
      IL_008d:  ldloc.0
      IL_008e:  ret
    } // end of method '<LocalInFinally>d__23`1'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       41 (0x29)
      .maxstack  2
      .locals init (!T V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::a
      IL_000d:  stloc.0
      IL_000e:  ldloca.s   V_0
      IL_0010:  constrained. !T
      IL_0016:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001b:  ldloca.s   V_0
      IL_001d:  constrained. !T
      IL_0023:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0028:  ret
    } // end of method '<LocalInFinally>d__23`1'::'<>m__Finally1'

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
      IL_0006:  ret
    } // end of method '<LocalInFinally>d__23`1'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<LocalInFinally>d__23`1'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<LocalInFinally>d__23`1'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>3__a'
      IL_0030:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::a
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<LocalInFinally>d__23`1'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<LocalInFinally>d__23`1'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<LocalInFinally>d__23`1'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'::System.Collections.IEnumerator.get_Current()
    } // end of property '<LocalInFinally>d__23`1'::System.Collections.IEnumerator.Current
  } // end of class '<LocalInFinally>d__23`1'

  .class auto ansi sealed nested private beforefieldinit '<GenericYield>d__24`1'<.ctor T>
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>,
                    [mscorlib]System.Collections.IEnumerable,
                    class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>,
                    [mscorlib]System.IDisposable,
                    [mscorlib]System.Collections.IEnumerator
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private int32 '<>1__state'
    .field private !T '<>2__current'
    .field private int32 '<>l__initialThreadId'
    .field private !T '<val>5__2'
    .field private int32 '<i>5__3'
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
      IL_0008:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<GenericYield>d__24`1'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<GenericYield>d__24`1'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       98 (0x62)
      .maxstack  3
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0010

      IL_000a:  ldloc.0
      IL_000b:  ldc.i4.1
      IL_000c:  beq.s      IL_0040

      IL_000e:  ldc.i4.0
      IL_000f:  ret

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.m1
      IL_0012:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0017:  ldarg.0
      IL_0018:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
      IL_001d:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<val>5__2'
      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.0
      IL_0024:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__3'
      IL_0029:  br.s       IL_0057

      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<val>5__2'
      IL_0032:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.m1
      IL_0042:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0047:  ldarg.0
      IL_0048:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__3'
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldloc.1
      IL_0050:  ldc.i4.1
      IL_0051:  add
      IL_0052:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__3'
      IL_0057:  ldarg.0
      IL_0058:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__3'
      IL_005d:  ldc.i4.3
      IL_005e:  blt.s      IL_002b

      IL_0060:  ldc.i4.0
      IL_0061:  ret
    } // end of method '<GenericYield>d__24`1'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance !T  'System.Collections.Generic.IEnumerator<T>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>2__current'
      IL_0006:  ret
    } // end of method '<GenericYield>d__24`1'::'System.Collections.Generic.IEnumerator<T>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<GenericYield>d__24`1'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>2__current'
      IL_0006:  box        !T
      IL_000b:  ret
    } // end of method '<GenericYield>d__24`1'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> 
            'System.Collections.Generic.IEnumerable<T>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>::GetEnumerator()
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ret
    } // end of method '<GenericYield>d__24`1'::'System.Collections.Generic.IEnumerable<T>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'System.Collections.Generic.IEnumerable<T>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<GenericYield>d__24`1'::System.Collections.IEnumerable.GetEnumerator

    .property instance !T 'System.Collections.Generic.IEnumerator<T>.Current'()
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'::'System.Collections.Generic.IEnumerator<T>.get_Current'()
    } // end of property '<GenericYield>d__24`1'::'System.Collections.Generic.IEnumerator<T>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'::System.Collections.IEnumerator.get_Current()
    } // end of property '<GenericYield>d__24`1'::System.Collections.IEnumerator.Current
  } // end of class '<GenericYield>d__24`1'

  .field private int32 fieldOnThis
  .method public hidebysig specialname static 
          class [mscorlib]System.Collections.Generic.IEnumerable`1<char> 
          get_YieldChars() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 58 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..XICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 67 65 74 5F 59 69 65 6C   // tyTest+<get_Yiel
                                                                                                                                          64 43 68 61 72 73 3E 64 5F 5F 32 00 00 )          // dChars>d__2..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::.ctor(int32)
    IL_0007:  ret
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
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..[ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 53 69 6D 70 6C 65 59 69   // tyTest+<SimpleYi
                                                                                                                                          65 6C 64 52 65 74 75 72 6E 3E 64 5F 5F 34 00 00 ) // eldReturn>d__4..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::SimpleYieldReturn

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
          SimpleYieldReturnEnumerator() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 65 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..eICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 53 69 6D 70 6C 65 59 69   // tyTest+<SimpleYi
                                                                                                                                          65 6C 64 52 65 74 75 72 6E 45 6E 75 6D 65 72 61   // eldReturnEnumera
                                                                                                                                          74 6F 72 3E 64 5F 5F 35 00 00 )                   // tor>d__5..
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::.ctor(int32)
    IL_0006:  ret
  } // end of method YieldReturnPrettyTest::SimpleYieldReturnEnumerator

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnParameters(int32 p) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5F 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // .._ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 50 61 72 61 6D 65 74 65 72 73 3E 64 5F   // urnParameters>d_
                                                                                                                                          5F 36 00 00 )                                     // _6..
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>4__this'
    IL_000e:  dup
    IL_000f:  ldarg.1
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>3__p'
    IL_0015:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnParameters

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
          YieldReturnParametersEnumerator(int32 p) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 69 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..iICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 50 61 72 61 6D 65 74 65 72 73 45 6E 75   // urnParametersEnu
                                                                                                                                          6D 65 72 61 74 6F 72 3E 64 5F 5F 37 00 00 )       // merator>d__7..
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::.ctor(int32)
    IL_0006:  dup
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>4__this'
    IL_000d:  dup
    IL_000e:  ldarg.1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::p
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnParametersEnumerator

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnInLoop() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..[ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 49 6E 4C 6F 6F 70 3E 64 5F 5F 38 00 00 ) // urnInLoop>d__8..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnInLoop

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnWithTryFinally() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 63 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..cICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 57 69 74 68 54 72 79 46 69 6E 61 6C 6C   // urnWithTryFinall
                                                                                                                                          79 3E 64 5F 5F 39 00 00 )                         // y>d__9..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          YieldReturnWithNestedTryFinally(bool breakInMiddle) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 6A 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..jICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 57 69 74 68 4E 65 73 74 65 64 54 72 79   // urnWithNestedTry
                                                                                                                                          46 69 6E 61 6C 6C 79 3E 64 5F 5F 31 30 00 00 )    // Finally>d__10..
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>3__breakInMiddle'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithNestedTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          YieldReturnWithTwoNonNestedFinallyBlocks(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 73 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..sICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 57 69 74 68 54 77 6F 4E 6F 6E 4E 65 73   // urnWithTwoNonNes
                                                                                                                                          74 65 64 46 69 6E 61 6C 6C 79 42 6C 6F 63 6B 73   // tedFinallyBlocks
                                                                                                                                          3E 64 5F 5F 31 31 00 00 )                         // >d__11..
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>3__input'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithTwoNonNestedFinallyBlocks

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> 
          YieldReturnWithAnonymousMethods1(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 6B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..kICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 57 69 74 68 41 6E 6F 6E 79 6D 6F 75 73   // urnWithAnonymous
                                                                                                                                          4D 65 74 68 6F 64 73 31 3E 64 5F 5F 31 32 00 00 ) // Methods1>d__12..
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>3__input'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithAnonymousMethods1

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> 
          YieldReturnWithAnonymousMethods2(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 6B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..kICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 52 65 74   // tyTest+<YieldRet
                                                                                                                                          75 72 6E 57 69 74 68 41 6E 6F 6E 79 6D 6F 75 73   // urnWithAnonymous
                                                                                                                                          4D 65 74 68 6F 64 73 32 3E 64 5F 5F 31 33 00 00 ) // Methods2>d__13..
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>3__input'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithAnonymousMethods2

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          GetEvenNumbers(int32 n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 59 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..YICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 47 65 74 45 76 65 6E 4E   // tyTest+<GetEvenN
                                                                                                                                          75 6D 62 65 72 73 3E 64 5F 5F 31 34 00 00 )       // umbers>d__14..
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>3__n'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::GetEvenNumbers

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<char> 
          ExceptionHandling() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..\ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 45 78 63 65 70 74 69 6F   // tyTest+<Exceptio
                                                                                                                                          6E 48 61 6E 64 6C 69 6E 67 3E 64 5F 5F 31 35 00   // nHandling>d__15.
                                                                                                                                          00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::ExceptionHandling

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInCatch() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..\ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 42 72 65   // tyTest+<YieldBre
                                                                                                                                          61 6B 49 6E 43 61 74 63 68 3E 64 5F 5F 31 36 00   // akInCatch>d__16.
                                                                                                                                          00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInCatch

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInCatchInTryFinally() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 68 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..hICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 42 72 65   // tyTest+<YieldBre
                                                                                                                                          61 6B 49 6E 43 61 74 63 68 49 6E 54 72 79 46 69   // akInCatchInTryFi
                                                                                                                                          6E 61 6C 6C 79 3E 64 5F 5F 31 37 00 00 )          // nally>d__17..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInCatchInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInTryCatchInTryFinally() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 6B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..kICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 42 72 65   // tyTest+<YieldBre
                                                                                                                                          61 6B 49 6E 54 72 79 43 61 74 63 68 49 6E 54 72   // akInTryCatchInTr
                                                                                                                                          79 46 69 6E 61 6C 6C 79 3E 64 5F 5F 31 38 00 00 ) // yFinally>d__18..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInTryCatchInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInTryFinallyInTryFinally(bool b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 6D 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..mICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 42 72 65   // tyTest+<YieldBre
                                                                                                                                          61 6B 49 6E 54 72 79 46 69 6E 61 6C 6C 79 49 6E   // akInTryFinallyIn
                                                                                                                                          54 72 79 46 69 6E 61 6C 6C 79 3E 64 5F 5F 31 39   // TryFinally>d__19
                                                                                                                                          00 00 ) 
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>3__b'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInTryFinallyInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakOnly() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 59 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..YICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 59 69 65 6C 64 42 72 65   // tyTest+<YieldBre
                                                                                                                                          61 6B 4F 6E 6C 79 3E 64 5F 5F 32 30 00 00 )       // akOnly>d__20..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakOnly

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          UnconditionalThrowInTryFinally() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 69 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..iICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 55 6E 63 6F 6E 64 69 74   // tyTest+<Uncondit
                                                                                                                                          69 6F 6E 61 6C 54 68 72 6F 77 49 6E 54 72 79 46   // ionalThrowInTryF
                                                                                                                                          69 6E 61 6C 6C 79 3E 64 5F 5F 32 31 00 00 )       // inally>d__21..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::UnconditionalThrowInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          NestedTryFinallyStartingOnSamePosition() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 71 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..qICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 4E 65 73 74 65 64 54 72   // tyTest+<NestedTr
                                                                                                                                          79 46 69 6E 61 6C 6C 79 53 74 61 72 74 69 6E 67   // yFinallyStarting
                                                                                                                                          4F 6E 53 61 6D 65 50 6F 73 69 74 69 6F 6E 3E 64   // OnSamePosition>d
                                                                                                                                          5F 5F 32 32 00 00 )                               // __22..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::.ctor(int32)
    IL_0007:  ret
  } // end of method YieldReturnPrettyTest::NestedTryFinallyStartingOnSamePosition

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          LocalInFinally<([mscorlib]System.IDisposable) T>(!!T a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5B 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..[ICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 4C 6F 63 61 6C 49 6E 46   // tyTest+<LocalInF
                                                                                                                                          69 6E 61 6C 6C 79 3E 64 5F 5F 32 33 60 31 00 00 ) // inally>d__23`1..
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!!T>::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!!T>::'<>3__a'
    IL_000e:  ret
  } // end of method YieldReturnPrettyTest::LocalInFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T> 
          GenericYield<.ctor T>() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 59 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..YICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 59 69 65 6C 64 52 65 74 75 72 6E 50 72 65 74   // .YieldReturnPret
                                                                                                                                          74 79 54 65 73 74 2B 3C 47 65 6E 65 72 69 63 59   // tyTest+<GenericY
                                                                                                                                          69 65 6C 64 3E 64 5F 5F 32 34 60 31 00 00 )       // ield>d__24`1..
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!!T>::.ctor(int32)
    IL_0007:  ret
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
  .class auto ansi sealed nested private beforefieldinit '<Count>d__1'
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
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn '<>4__this'
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn '<>3__<>4__this'
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
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_000d:  ldarg.0
      IL_000e:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>l__initialThreadId'
      IL_0018:  ret
    } // end of method '<Count>d__1'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<Count>d__1'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       134 (0x86)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_004c,
                            IL_007d)
      IL_0019:  ldc.i4.0
      IL_001a:  ret

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.m1
      IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0022:  ldarg.0
      IL_0023:  ldarg.0
      IL_0024:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0029:  ldarg.0
      IL_002a:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_002f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0034:  stloc.1
      IL_0035:  ldloc.1
      IL_0036:  ldc.i4.1
      IL_0037:  add
      IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_003d:  ldloc.1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>2__current'
      IL_0043:  ldarg.0
      IL_0044:  ldc.i4.1
      IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_004a:  ldc.i4.1
      IL_004b:  ret

      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.m1
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0053:  ldarg.0
      IL_0054:  ldarg.0
      IL_0055:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_005a:  ldarg.0
      IL_005b:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0060:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0065:  stloc.1
      IL_0066:  ldloc.1
      IL_0067:  ldc.i4.1
      IL_0068:  add
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_006e:  ldloc.1
      IL_006f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>2__current'
      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.2
      IL_0076:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_007b:  ldc.i4.1
      IL_007c:  ret

      IL_007d:  ldarg.0
      IL_007e:  ldc.i4.m1
      IL_007f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0084:  ldc.i4.0
      IL_0085:  ret
    } // end of method '<Count>d__1'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>2__current'
      IL_0006:  ret
    } // end of method '<Count>d__1'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'

    .method private hidebysig newslot virtual final 
            instance void  System.Collections.IEnumerator.Reset() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::Reset
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotSupportedException::.ctor()
      IL_0005:  throw
    } // end of method '<Count>d__1'::System.Collections.IEnumerator.Reset

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  ret
    } // end of method '<Count>d__1'::System.Collections.IEnumerator.get_Current

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1' V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0006:  ldc.i4.s   -2
      IL_0008:  bne.un.s   IL_0022

      IL_000a:  ldarg.0
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>l__initialThreadId'
      IL_0010:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0015:  bne.un.s   IL_0022

      IL_0017:  ldarg.0
      IL_0018:  ldc.i4.0
      IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_001e:  ldarg.0
      IL_001f:  stloc.0
      IL_0020:  br.s       IL_0029

      IL_0022:  ldc.i4.0
      IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::.ctor(int32)
      IL_0028:  stloc.0
      IL_0029:  ldloc.0
      IL_002a:  ldarg.0
      IL_002b:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>3__<>4__this'
      IL_0030:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0035:  ldloc.0
      IL_0036:  ret
    } // end of method '<Count>d__1'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  ret
    } // end of method '<Count>d__1'::System.Collections.IEnumerable.GetEnumerator

    .property instance int32 'System.Collections.Generic.IEnumerator<System.Int32>.Current'()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'()
    } // end of property '<Count>d__1'::'System.Collections.Generic.IEnumerator<System.Int32>.Current'
    .property instance object System.Collections.IEnumerator.Current()
    {
      .get instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::System.Collections.IEnumerator.get_Current()
    } // end of property '<Count>d__1'::System.Collections.IEnumerator.Current
  } // end of class '<Count>d__1'

  .field private int32 val
  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          Count() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.IteratorStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4F 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..OICSharpCode.D
                                                                                                                                          65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                          54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                          2E 53 74 72 75 63 74 57 69 74 68 59 69 65 6C 64   // .StructWithYield
                                                                                                                                          52 65 74 75 72 6E 2B 3C 43 6F 75 6E 74 3E 64 5F   // Return+<Count>d_
                                                                                                                                          5F 31 00 00 )                                     // _1..
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::.ctor(int32)
    IL_0007:  dup
    IL_0008:  ldarg.0
    IL_0009:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn
    IL_000e:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>3__<>4__this'
    IL_0013:  ret
  } // end of method StructWithYieldReturn::Count

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
