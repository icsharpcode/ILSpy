



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       123 (0x7b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_0021,
                            IL_0023,
                            IL_0025)
      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_0029

      IL_0021:  br.s       IL_0042

      IL_0023:  br.s       IL_005a

      IL_0025:  br.s       IL_0072

      IL_0027:  ldc.i4.0
      IL_0028:  ret

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.m1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldc.i4.s   97
      IL_0034:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_0039:  ldarg.0
      IL_003a:  ldc.i4.1
      IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0040:  ldc.i4.1
      IL_0041:  ret

      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.m1
      IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0049:  ldarg.0
      IL_004a:  ldc.i4.s   98
      IL_004c:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_0051:  ldarg.0
      IL_0052:  ldc.i4.2
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0058:  ldc.i4.1
      IL_0059:  ret

      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.m1
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0061:  ldarg.0
      IL_0062:  ldc.i4.s   99
      IL_0064:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>2__current'
      IL_0069:  ldarg.0
      IL_006a:  ldc.i4.3
      IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0070:  ldc.i4.1
      IL_0071:  ret

      IL_0072:  ldarg.0
      IL_0073:  ldc.i4.m1
      IL_0074:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::'<>1__state'
      IL_0079:  ldc.i4.0
      IL_007a:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       132 (0x84)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_0021,
                            IL_0023,
                            IL_0025)
      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_0029

      IL_0021:  br.s       IL_0045

      IL_0023:  br.s       IL_0060

      IL_0025:  br.s       IL_007b

      IL_0027:  ldc.i4.0
      IL_0028:  ret

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.m1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldstr      "A"
      IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0043:  ldc.i4.1
      IL_0044:  ret

      IL_0045:  ldarg.0
      IL_0046:  ldc.i4.m1
      IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_004c:  ldarg.0
      IL_004d:  ldstr      "B"
      IL_0052:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_005e:  ldc.i4.1
      IL_005f:  ret

      IL_0060:  ldarg.0
      IL_0061:  ldc.i4.m1
      IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0067:  ldarg.0
      IL_0068:  ldstr      "C"
      IL_006d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>2__current'
      IL_0072:  ldarg.0
      IL_0073:  ldc.i4.3
      IL_0074:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0079:  ldc.i4.1
      IL_007a:  ret

      IL_007b:  ldarg.0
      IL_007c:  ldc.i4.m1
      IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__4'::'<>1__state'
      IL_0082:  ldc.i4.0
      IL_0083:  ret
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
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_000e:  ret
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
      // Code size       132 (0x84)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_0021,
                            IL_0023,
                            IL_0025)
      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_0029

      IL_0021:  br.s       IL_0045

      IL_0023:  br.s       IL_0060

      IL_0025:  br.s       IL_007b

      IL_0027:  ldc.i4.0
      IL_0028:  ret

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.m1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldstr      "A"
      IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0043:  ldc.i4.1
      IL_0044:  ret

      IL_0045:  ldarg.0
      IL_0046:  ldc.i4.m1
      IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_004c:  ldarg.0
      IL_004d:  ldstr      "B"
      IL_0052:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_005e:  ldc.i4.1
      IL_005f:  ret

      IL_0060:  ldarg.0
      IL_0061:  ldc.i4.m1
      IL_0062:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0067:  ldarg.0
      IL_0068:  ldstr      "C"
      IL_006d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>2__current'
      IL_0072:  ldarg.0
      IL_0073:  ldc.i4.3
      IL_0074:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0079:  ldc.i4.1
      IL_007a:  ret

      IL_007b:  ldarg.0
      IL_007c:  ldc.i4.m1
      IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__5'::'<>1__state'
      IL_0082:  ldc.i4.0
      IL_0083:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       106 (0x6a)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_0040

      IL_001f:  br.s       IL_0061

      IL_0021:  ldc.i4.0
      IL_0022:  ret

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::p
      IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.m1
      IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0047:  ldarg.0
      IL_0048:  ldarg.0
      IL_0049:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>4__this'
      IL_004e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>2__current'
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_005f:  ldc.i4.1
      IL_0060:  ret

      IL_0061:  ldarg.0
      IL_0062:  ldc.i4.m1
      IL_0063:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__6'::'<>1__state'
      IL_0068:  ldc.i4.0
      IL_0069:  ret
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
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_000e:  ret
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
      // Code size       106 (0x6a)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_0040

      IL_001f:  br.s       IL_0061

      IL_0021:  ldc.i4.0
      IL_0022:  ret

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::p
      IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  ret

      IL_0040:  ldarg.0
      IL_0041:  ldc.i4.m1
      IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0047:  ldarg.0
      IL_0048:  ldarg.0
      IL_0049:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>4__this'
      IL_004e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>2__current'
      IL_0058:  ldarg.0
      IL_0059:  ldc.i4.2
      IL_005a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_005f:  ldc.i4.1
      IL_0060:  ret

      IL_0061:  ldarg.0
      IL_0062:  ldc.i4.m1
      IL_0063:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__7'::'<>1__state'
      IL_0068:  ldc.i4.0
      IL_0069:  ret
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
    .field private int32 '<i>5__1'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       103 (0x67)
      .maxstack  3
      .locals init (int32 V_0,
               int32 V_1,
               bool V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  br.s       IL_000c

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  beq.s      IL_0014

      IL_0010:  br.s       IL_0016

      IL_0012:  br.s       IL_0018

      IL_0014:  br.s       IL_003f

      IL_0016:  ldc.i4.0
      IL_0017:  ret

      IL_0018:  ldarg.0
      IL_0019:  ldc.i4.m1
      IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_001f:  nop
      IL_0020:  ldarg.0
      IL_0021:  ldc.i4.0
      IL_0022:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__1'
      IL_0027:  br.s       IL_0057

      IL_0029:  nop
      IL_002a:  ldarg.0
      IL_002b:  ldarg.0
      IL_002c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__1'
      IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>2__current'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_003d:  ldc.i4.1
      IL_003e:  ret

      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.m1
      IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<>1__state'
      IL_0046:  nop
      IL_0047:  ldarg.0
      IL_0048:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__1'
      IL_004d:  stloc.1
      IL_004e:  ldarg.0
      IL_004f:  ldloc.1
      IL_0050:  ldc.i4.1
      IL_0051:  add
      IL_0052:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__1'
      IL_0057:  ldarg.0
      IL_0058:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__8'::'<i>5__1'
      IL_005d:  ldc.i4.s   100
      IL_005f:  clt
      IL_0061:  stloc.2
      IL_0062:  ldloc.2
      IL_0063:  brtrue.s   IL_0029

      IL_0065:  ldc.i4.0
      IL_0066:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0014

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.2
      IL_0010:  beq.s      IL_0014

      IL_0012:  br.s       IL_0020

      IL_0014:  nop
      .try
      {
        IL_0015:  leave.s    IL_001e

      }  // end .try
      finally
      {
        IL_0017:  ldarg.0
        IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>m__Finally1'()
        IL_001d:  endfinally
      }  // end handler
      IL_001e:  br.s       IL_0020

      IL_0020:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       158 (0x9e)
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
                              IL_001f,
                              IL_0021,
                              IL_0023,
                              IL_0025)
        IL_001d:  br.s       IL_0027

        IL_001f:  br.s       IL_002b

        IL_0021:  br.s       IL_0045

        IL_0023:  br.s       IL_0067

        IL_0025:  br.s       IL_0089

        IL_0027:  ldc.i4.0
        IL_0028:  stloc.0
        IL_0029:  leave.s    IL_009c

        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.m1
        IL_002d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0032:  nop
        IL_0033:  ldarg.0
        IL_0034:  ldc.i4.0
        IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
        IL_003a:  ldarg.0
        IL_003b:  ldc.i4.1
        IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0041:  ldc.i4.1
        IL_0042:  stloc.0
        IL_0043:  leave.s    IL_009c

        IL_0045:  ldarg.0
        IL_0046:  ldc.i4.m1
        IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_004c:  ldarg.0
        IL_004d:  ldc.i4.s   -3
        IL_004f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0054:  nop
        IL_0055:  ldarg.0
        IL_0056:  ldc.i4.1
        IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
        IL_005c:  ldarg.0
        IL_005d:  ldc.i4.2
        IL_005e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0063:  ldc.i4.1
        IL_0064:  stloc.0
        IL_0065:  leave.s    IL_009c

        IL_0067:  ldarg.0
        IL_0068:  ldc.i4.s   -3
        IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_006f:  nop
        IL_0070:  ldarg.0
        IL_0071:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>m__Finally1'()
        IL_0076:  nop
        IL_0077:  ldarg.0
        IL_0078:  ldc.i4.2
        IL_0079:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>2__current'
        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.3
        IL_0080:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0085:  ldc.i4.1
        IL_0086:  stloc.0
        IL_0087:  leave.s    IL_009c

        IL_0089:  ldarg.0
        IL_008a:  ldc.i4.m1
        IL_008b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
        IL_0090:  ldc.i4.0
        IL_0091:  stloc.0
        IL_0092:  leave.s    IL_009c

      }  // end .try
      fault
      {
        IL_0094:  ldarg.0
        IL_0095:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::System.IDisposable.Dispose()
        IL_009a:  nop
        IL_009b:  endfinally
      }  // end handler
      IL_009c:  ldloc.0
      IL_009d:  ret
    } // end of method '<YieldReturnWithTryFinally>d__9'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__9'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Finally!"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       64 (0x40)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -4
      IL_000a:  sub
      IL_000b:  ldc.i4.1
      IL_000c:  ble.un.s   IL_0018

      IL_000e:  br.s       IL_0010

      IL_0010:  ldloc.0
      IL_0011:  ldc.i4.2
      IL_0012:  sub
      IL_0013:  ldc.i4.3
      IL_0014:  ble.un.s   IL_0018

      IL_0016:  br.s       IL_003f

      IL_0018:  nop
      .try
      {
        IL_0019:  ldloc.0
        IL_001a:  ldc.i4.s   -4
        IL_001c:  beq.s      IL_0028

        IL_001e:  br.s       IL_0020

        IL_0020:  ldloc.0
        IL_0021:  ldc.i4.3
        IL_0022:  sub
        IL_0023:  ldc.i4.1
        IL_0024:  ble.un.s   IL_0028

        IL_0026:  br.s       IL_0034

        IL_0028:  nop
        .try
        {
          IL_0029:  leave.s    IL_0032

        }  // end .try
        finally
        {
          IL_002b:  ldarg.0
          IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'()
          IL_0031:  endfinally
        }  // end handler
        IL_0032:  br.s       IL_0034

        IL_0034:  leave.s    IL_003d

      }  // end .try
      finally
      {
        IL_0036:  ldarg.0
        IL_0037:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'()
        IL_003c:  endfinally
      }  // end handler
      IL_003d:  br.s       IL_003f

      IL_003f:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       505 (0x1f9)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_002b,
                              IL_002d,
                              IL_002f,
                              IL_0034,
                              IL_0039,
                              IL_003e,
                              IL_0043)
        IL_0029:  br.s       IL_0048

        IL_002b:  br.s       IL_004f

        IL_002d:  br.s       IL_007b

        IL_002f:  br         IL_00ba

        IL_0034:  br         IL_00fa

        IL_0039:  br         IL_014b

        IL_003e:  br         IL_0192

        IL_0043:  br         IL_01d9

        IL_0048:  ldc.i4.0
        IL_0049:  stloc.0
        IL_004a:  leave      IL_01f7

        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.m1
        IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0056:  nop
        IL_0057:  ldstr      "Start of method - 1"
        IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0061:  nop
        IL_0062:  ldarg.0
        IL_0063:  ldstr      "Start of method"
        IL_0068:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_006d:  ldarg.0
        IL_006e:  ldc.i4.1
        IL_006f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0074:  ldc.i4.1
        IL_0075:  stloc.0
        IL_0076:  leave      IL_01f7

        IL_007b:  ldarg.0
        IL_007c:  ldc.i4.m1
        IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0082:  ldstr      "Start of method - 2"
        IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_008c:  nop
        IL_008d:  ldarg.0
        IL_008e:  ldc.i4.s   -3
        IL_0090:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0095:  nop
        IL_0096:  ldstr      "Within outer try - 1"
        IL_009b:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00a0:  nop
        IL_00a1:  ldarg.0
        IL_00a2:  ldstr      "Within outer try"
        IL_00a7:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_00ac:  ldarg.0
        IL_00ad:  ldc.i4.2
        IL_00ae:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00b3:  ldc.i4.1
        IL_00b4:  stloc.0
        IL_00b5:  leave      IL_01f7

        IL_00ba:  ldarg.0
        IL_00bb:  ldc.i4.s   -3
        IL_00bd:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00c2:  ldstr      "Within outer try - 2"
        IL_00c7:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00cc:  nop
        IL_00cd:  ldarg.0
        IL_00ce:  ldc.i4.s   -4
        IL_00d0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00d5:  nop
        IL_00d6:  ldstr      "Within inner try - 1"
        IL_00db:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00e0:  nop
        IL_00e1:  ldarg.0
        IL_00e2:  ldstr      "Within inner try"
        IL_00e7:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_00ec:  ldarg.0
        IL_00ed:  ldc.i4.3
        IL_00ee:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_00f3:  ldc.i4.1
        IL_00f4:  stloc.0
        IL_00f5:  leave      IL_01f7

        IL_00fa:  ldarg.0
        IL_00fb:  ldc.i4.s   -4
        IL_00fd:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0102:  ldstr      "Within inner try - 2"
        IL_0107:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_010c:  nop
        IL_010d:  ldarg.0
        IL_010e:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::breakInMiddle
        IL_0113:  stloc.2
        IL_0114:  ldloc.2
        IL_0115:  brfalse.s  IL_0127

        IL_0117:  nop
        IL_0118:  ldstr      "Breaking..."
        IL_011d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0122:  nop
        IL_0123:  ldc.i4.0
        IL_0124:  stloc.0
        IL_0125:  br.s       IL_0168

        IL_0127:  ldstr      "End of inner try - 1"
        IL_012c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0131:  nop
        IL_0132:  ldarg.0
        IL_0133:  ldstr      "End of inner try"
        IL_0138:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_013d:  ldarg.0
        IL_013e:  ldc.i4.4
        IL_013f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0144:  ldc.i4.1
        IL_0145:  stloc.0
        IL_0146:  leave      IL_01f7

        IL_014b:  ldarg.0
        IL_014c:  ldc.i4.s   -4
        IL_014e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_0153:  ldstr      "End of inner try - 2"
        IL_0158:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_015d:  nop
        IL_015e:  nop
        IL_015f:  ldarg.0
        IL_0160:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'()
        IL_0165:  nop
        IL_0166:  br.s       IL_0171

        IL_0168:  ldarg.0
        IL_0169:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally2'()
        IL_016e:  nop
        IL_016f:  br.s       IL_01af

        IL_0171:  ldstr      "End of outer try - 1"
        IL_0176:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_017b:  nop
        IL_017c:  ldarg.0
        IL_017d:  ldstr      "End of outer try"
        IL_0182:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_0187:  ldarg.0
        IL_0188:  ldc.i4.5
        IL_0189:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_018e:  ldc.i4.1
        IL_018f:  stloc.0
        IL_0190:  leave.s    IL_01f7

        IL_0192:  ldarg.0
        IL_0193:  ldc.i4.s   -3
        IL_0195:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_019a:  ldstr      "End of outer try - 2"
        IL_019f:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01a4:  nop
        IL_01a5:  nop
        IL_01a6:  ldarg.0
        IL_01a7:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'()
        IL_01ac:  nop
        IL_01ad:  br.s       IL_01b8

        IL_01af:  ldarg.0
        IL_01b0:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'()
        IL_01b5:  nop
        IL_01b6:  leave.s    IL_01f7

        IL_01b8:  ldstr      "End of method - 1"
        IL_01bd:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01c2:  nop
        IL_01c3:  ldarg.0
        IL_01c4:  ldstr      "End of method"
        IL_01c9:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>2__current'
        IL_01ce:  ldarg.0
        IL_01cf:  ldc.i4.6
        IL_01d0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_01d5:  ldc.i4.1
        IL_01d6:  stloc.0
        IL_01d7:  leave.s    IL_01f7

        IL_01d9:  ldarg.0
        IL_01da:  ldc.i4.m1
        IL_01db:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
        IL_01e0:  ldstr      "End of method - 2"
        IL_01e5:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01ea:  nop
        IL_01eb:  ldc.i4.0
        IL_01ec:  stloc.0
        IL_01ed:  leave.s    IL_01f7

      }  // end .try
      fault
      {
        IL_01ef:  ldarg.0
        IL_01f0:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::System.IDisposable.Dispose()
        IL_01f5:  nop
        IL_01f6:  endfinally
      }  // end handler
      IL_01f7:  ldloc.0
      IL_01f8:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Outer Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__10'::'<>m__Finally1'

    .method private hidebysig instance void 
            '<>m__Finally2'() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   -3
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__10'::'<>1__state'
      IL_0008:  nop
      IL_0009:  ldstr      "Inner Finally"
      IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0013:  nop
      IL_0014:  nop
      IL_0015:  ret
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
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>s__1'
    .field private string '<line>5__2'
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>s__3'
    .field private string '<item>5__4'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       102 (0x66)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -5
      IL_000a:  sub
      IL_000b:  switch     ( 
                            IL_0059,
                            IL_0034,
                            IL_0034,
                            IL_0065,
                            IL_0065,
                            IL_0065,
                            IL_0034)
      IL_002c:  br.s       IL_002e

      IL_002e:  ldloc.0
      IL_002f:  ldc.i4.8
      IL_0030:  beq.s      IL_0059

      IL_0032:  br.s       IL_0065

      IL_0034:  nop
      .try
      {
        IL_0035:  ldloc.0
        IL_0036:  ldc.i4.s   -4
        IL_0038:  beq.s      IL_0042

        IL_003a:  br.s       IL_003c

        IL_003c:  ldloc.0
        IL_003d:  ldc.i4.1
        IL_003e:  beq.s      IL_0042

        IL_0040:  br.s       IL_004e

        IL_0042:  nop
        .try
        {
          IL_0043:  leave.s    IL_004c

        }  // end .try
        finally
        {
          IL_0045:  ldarg.0
          IL_0046:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally2'()
          IL_004b:  endfinally
        }  // end handler
        IL_004c:  br.s       IL_004e

        IL_004e:  leave.s    IL_0057

      }  // end .try
      finally
      {
        IL_0050:  ldarg.0
        IL_0051:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally1'()
        IL_0056:  endfinally
      }  // end handler
      IL_0057:  br.s       IL_0065

      IL_0059:  nop
      .try
      {
        IL_005a:  leave.s    IL_0063

      }  // end .try
      finally
      {
        IL_005c:  ldarg.0
        IL_005d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally3'()
        IL_0062:  endfinally
      }  // end handler
      IL_0063:  br.s       IL_0065

      IL_0065:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       563 (0x233)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0033,
                              IL_0035,
                              IL_003a,
                              IL_003f,
                              IL_0044,
                              IL_0049,
                              IL_004e,
                              IL_0053,
                              IL_0058)
        IL_0031:  br.s       IL_005d

        IL_0033:  br.s       IL_0064

        IL_0035:  br         IL_00bd

        IL_003a:  br         IL_0109

        IL_003f:  br         IL_0129

        IL_0044:  br         IL_0149

        IL_0049:  br         IL_0169

        IL_004e:  br         IL_0189

        IL_0053:  br         IL_01a9

        IL_0058:  br         IL_01fa

        IL_005d:  ldc.i4.0
        IL_005e:  stloc.0
        IL_005f:  leave      IL_0231

        IL_0064:  ldarg.0
        IL_0065:  ldc.i4.m1
        IL_0066:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_006b:  nop
        IL_006c:  nop
        IL_006d:  ldarg.0
        IL_006e:  ldarg.0
        IL_006f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::input
        IL_0074:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0079:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__1'
        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.s   -3
        IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0086:  br.s       IL_00d5

        IL_0088:  ldarg.0
        IL_0089:  ldarg.0
        IL_008a:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__1'
        IL_008f:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_0094:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__2'
        IL_0099:  nop
        IL_009a:  ldarg.0
        IL_009b:  ldc.i4.s   -4
        IL_009d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00a2:  nop
        IL_00a3:  ldarg.0
        IL_00a4:  ldarg.0
        IL_00a5:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__2'
        IL_00aa:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_00af:  ldarg.0
        IL_00b0:  ldc.i4.1
        IL_00b1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00b6:  ldc.i4.1
        IL_00b7:  stloc.0
        IL_00b8:  leave      IL_0231

        IL_00bd:  ldarg.0
        IL_00be:  ldc.i4.s   -4
        IL_00c0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_00c5:  nop
        IL_00c6:  ldarg.0
        IL_00c7:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally2'()
        IL_00cc:  nop
        IL_00cd:  nop
        IL_00ce:  ldarg.0
        IL_00cf:  ldnull
        IL_00d0:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__2'
        IL_00d5:  ldarg.0
        IL_00d6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__1'
        IL_00db:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00e0:  brtrue.s   IL_0088

        IL_00e2:  ldarg.0
        IL_00e3:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally1'()
        IL_00e8:  nop
        IL_00e9:  ldarg.0
        IL_00ea:  ldnull
        IL_00eb:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__1'
        IL_00f0:  ldarg.0
        IL_00f1:  ldstr      "A"
        IL_00f6:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_00fb:  ldarg.0
        IL_00fc:  ldc.i4.2
        IL_00fd:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0102:  ldc.i4.1
        IL_0103:  stloc.0
        IL_0104:  leave      IL_0231

        IL_0109:  ldarg.0
        IL_010a:  ldc.i4.m1
        IL_010b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0110:  ldarg.0
        IL_0111:  ldstr      "B"
        IL_0116:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_011b:  ldarg.0
        IL_011c:  ldc.i4.3
        IL_011d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0122:  ldc.i4.1
        IL_0123:  stloc.0
        IL_0124:  leave      IL_0231

        IL_0129:  ldarg.0
        IL_012a:  ldc.i4.m1
        IL_012b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0130:  ldarg.0
        IL_0131:  ldstr      "C"
        IL_0136:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_013b:  ldarg.0
        IL_013c:  ldc.i4.4
        IL_013d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0142:  ldc.i4.1
        IL_0143:  stloc.0
        IL_0144:  leave      IL_0231

        IL_0149:  ldarg.0
        IL_014a:  ldc.i4.m1
        IL_014b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0150:  ldarg.0
        IL_0151:  ldstr      "D"
        IL_0156:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_015b:  ldarg.0
        IL_015c:  ldc.i4.5
        IL_015d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0162:  ldc.i4.1
        IL_0163:  stloc.0
        IL_0164:  leave      IL_0231

        IL_0169:  ldarg.0
        IL_016a:  ldc.i4.m1
        IL_016b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0170:  ldarg.0
        IL_0171:  ldstr      "E"
        IL_0176:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_017b:  ldarg.0
        IL_017c:  ldc.i4.6
        IL_017d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0182:  ldc.i4.1
        IL_0183:  stloc.0
        IL_0184:  leave      IL_0231

        IL_0189:  ldarg.0
        IL_018a:  ldc.i4.m1
        IL_018b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0190:  ldarg.0
        IL_0191:  ldstr      "F"
        IL_0196:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_019b:  ldarg.0
        IL_019c:  ldc.i4.7
        IL_019d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_01a2:  ldc.i4.1
        IL_01a3:  stloc.0
        IL_01a4:  leave      IL_0231

        IL_01a9:  ldarg.0
        IL_01aa:  ldc.i4.m1
        IL_01ab:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_01b0:  nop
        IL_01b1:  ldarg.0
        IL_01b2:  ldarg.0
        IL_01b3:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::input
        IL_01b8:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_01bd:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__3'
        IL_01c2:  ldarg.0
        IL_01c3:  ldc.i4.s   -5
        IL_01c5:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_01ca:  br.s       IL_020a

        IL_01cc:  ldarg.0
        IL_01cd:  ldarg.0
        IL_01ce:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__3'
        IL_01d3:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_01d8:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<item>5__4'
        IL_01dd:  nop
        IL_01de:  ldarg.0
        IL_01df:  ldarg.0
        IL_01e0:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<item>5__4'
        IL_01e5:  callvirt   instance string [mscorlib]System.String::ToUpper()
        IL_01ea:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>2__current'
        IL_01ef:  ldarg.0
        IL_01f0:  ldc.i4.8
        IL_01f1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_01f6:  ldc.i4.1
        IL_01f7:  stloc.0
        IL_01f8:  leave.s    IL_0231

        IL_01fa:  ldarg.0
        IL_01fb:  ldc.i4.s   -5
        IL_01fd:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
        IL_0202:  nop
        IL_0203:  ldarg.0
        IL_0204:  ldnull
        IL_0205:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<item>5__4'
        IL_020a:  ldarg.0
        IL_020b:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__3'
        IL_0210:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0215:  brtrue.s   IL_01cc

        IL_0217:  ldarg.0
        IL_0218:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally3'()
        IL_021d:  nop
        IL_021e:  ldarg.0
        IL_021f:  ldnull
        IL_0220:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__3'
        IL_0225:  ldc.i4.0
        IL_0226:  stloc.0
        IL_0227:  leave.s    IL_0231

      }  // end .try
      fault
      {
        IL_0229:  ldarg.0
        IL_022a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::System.IDisposable.Dispose()
        IL_022f:  nop
        IL_0230:  endfinally
      }  // end handler
      IL_0231:  ldloc.0
      IL_0232:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__1'
      IL_000d:  brfalse.s  IL_001b

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  nop
      IL_001b:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally1'

    .method private hidebysig instance void 
            '<>m__Finally2'() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   -3
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0008:  nop
      IL_0009:  ldstr      "Processed "
      IL_000e:  ldarg.0
      IL_000f:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<line>5__2'
      IL_0014:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>m__Finally2'

    .method private hidebysig instance void 
            '<>m__Finally3'() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__3'
      IL_000d:  brfalse.s  IL_001b

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__11'::'<>s__3'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  nop
      IL_001b:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>s__1'
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0' '<>8__2'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0014

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  beq.s      IL_0014

      IL_0012:  br.s       IL_0020

      IL_0014:  nop
      .try
      {
        IL_0015:  leave.s    IL_001e

      }  // end .try
      finally
      {
        IL_0017:  ldarg.0
        IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>m__Finally1'()
        IL_001d:  endfinally
      }  // end handler
      IL_001e:  br.s       IL_0020

      IL_0020:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       190 (0xbe)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.1
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0016

        IL_0012:  br.s       IL_001d

        IL_0014:  br.s       IL_0085

        IL_0016:  ldc.i4.0
        IL_0017:  stloc.0
        IL_0018:  leave      IL_00bc

        IL_001d:  ldarg.0
        IL_001e:  ldc.i4.m1
        IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0024:  nop
        IL_0025:  nop
        IL_0026:  ldarg.0
        IL_0027:  ldarg.0
        IL_0028:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::input
        IL_002d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0032:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>s__1'
        IL_0037:  ldarg.0
        IL_0038:  ldc.i4.s   -3
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_003f:  br.s       IL_0095

        IL_0041:  ldarg.0
        IL_0042:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::.ctor()
        IL_0047:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>8__2'
        IL_004c:  ldarg.0
        IL_004d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>8__2'
        IL_0052:  ldarg.0
        IL_0053:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>s__1'
        IL_0058:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_005d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::line
        IL_0062:  nop
        IL_0063:  ldarg.0
        IL_0064:  ldarg.0
        IL_0065:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>8__2'
        IL_006a:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0'::'<YieldReturnWithAnonymousMethods1>b__0'()
        IL_0070:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_0075:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>2__current'
        IL_007a:  ldarg.0
        IL_007b:  ldc.i4.1
        IL_007c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_0081:  ldc.i4.1
        IL_0082:  stloc.0
        IL_0083:  leave.s    IL_00bc

        IL_0085:  ldarg.0
        IL_0086:  ldc.i4.s   -3
        IL_0088:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
        IL_008d:  nop
        IL_008e:  ldarg.0
        IL_008f:  ldnull
        IL_0090:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass12_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>8__2'
        IL_0095:  ldarg.0
        IL_0096:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>s__1'
        IL_009b:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00a0:  brtrue.s   IL_0041

        IL_00a2:  ldarg.0
        IL_00a3:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>m__Finally1'()
        IL_00a8:  nop
        IL_00a9:  ldarg.0
        IL_00aa:  ldnull
        IL_00ab:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>s__1'
        IL_00b0:  ldc.i4.0
        IL_00b1:  stloc.0
        IL_00b2:  leave.s    IL_00bc

      }  // end .try
      fault
      {
        IL_00b4:  ldarg.0
        IL_00b5:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::System.IDisposable.Dispose()
        IL_00ba:  nop
        IL_00bb:  endfinally
      }  // end handler
      IL_00bc:  ldloc.0
      IL_00bd:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__12'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>s__1'
      IL_000d:  brfalse.s  IL_001b

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__12'::'<>s__1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  nop
      IL_001b:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
    .field private class [mscorlib]System.Collections.Generic.IEnumerator`1<string> '<>s__1'
    .field private string '<item>5__2'
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0' '<>8__3'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0014

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  beq.s      IL_0014

      IL_0012:  br.s       IL_0020

      IL_0014:  nop
      .try
      {
        IL_0015:  leave.s    IL_001e

      }  // end .try
      finally
      {
        IL_0017:  ldarg.0
        IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>m__Finally1'()
        IL_001d:  endfinally
      }  // end handler
      IL_001e:  br.s       IL_0020

      IL_0020:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       209 (0xd1)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.1
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0016

        IL_0012:  br.s       IL_001d

        IL_0014:  br.s       IL_0091

        IL_0016:  ldc.i4.0
        IL_0017:  stloc.0
        IL_0018:  leave      IL_00cf

        IL_001d:  ldarg.0
        IL_001e:  ldc.i4.m1
        IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0024:  nop
        IL_0025:  nop
        IL_0026:  ldarg.0
        IL_0027:  ldarg.0
        IL_0028:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::input
        IL_002d:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0032:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>s__1'
        IL_0037:  ldarg.0
        IL_0038:  ldc.i4.s   -3
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_003f:  br.s       IL_00a8

        IL_0041:  ldarg.0
        IL_0042:  ldarg.0
        IL_0043:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>s__1'
        IL_0048:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_004d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<item>5__2'
        IL_0052:  ldarg.0
        IL_0053:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::.ctor()
        IL_0058:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>8__3'
        IL_005d:  nop
        IL_005e:  ldarg.0
        IL_005f:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>8__3'
        IL_0064:  ldarg.0
        IL_0065:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<item>5__2'
        IL_006a:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::copy
        IL_006f:  ldarg.0
        IL_0070:  ldarg.0
        IL_0071:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>8__3'
        IL_0076:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0'::'<YieldReturnWithAnonymousMethods2>b__0'()
        IL_007c:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_0081:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>2__current'
        IL_0086:  ldarg.0
        IL_0087:  ldc.i4.1
        IL_0088:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_008d:  ldc.i4.1
        IL_008e:  stloc.0
        IL_008f:  leave.s    IL_00cf

        IL_0091:  ldarg.0
        IL_0092:  ldc.i4.s   -3
        IL_0094:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
        IL_0099:  nop
        IL_009a:  ldarg.0
        IL_009b:  ldnull
        IL_009c:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass13_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>8__3'
        IL_00a1:  ldarg.0
        IL_00a2:  ldnull
        IL_00a3:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<item>5__2'
        IL_00a8:  ldarg.0
        IL_00a9:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>s__1'
        IL_00ae:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00b3:  brtrue.s   IL_0041

        IL_00b5:  ldarg.0
        IL_00b6:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>m__Finally1'()
        IL_00bb:  nop
        IL_00bc:  ldarg.0
        IL_00bd:  ldnull
        IL_00be:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>s__1'
        IL_00c3:  ldc.i4.0
        IL_00c4:  stloc.0
        IL_00c5:  leave.s    IL_00cf

      }  // end .try
      fault
      {
        IL_00c7:  ldarg.0
        IL_00c8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::System.IDisposable.Dispose()
        IL_00cd:  nop
        IL_00ce:  endfinally
      }  // end handler
      IL_00cf:  ldloc.0
      IL_00d0:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__13'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>s__1'
      IL_000d:  brfalse.s  IL_001b

      IL_000f:  ldarg.0
      IL_0010:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__13'::'<>s__1'
      IL_0015:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001a:  nop
      IL_001b:  ret
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
    .field private int32 '<i>5__1'
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
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       124 (0x7c)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1,
               int32 V_2,
               bool V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  br.s       IL_000c

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  beq.s      IL_0014

      IL_0010:  br.s       IL_0016

      IL_0012:  br.s       IL_0018

      IL_0014:  br.s       IL_004f

      IL_0016:  ldc.i4.0
      IL_0017:  ret

      IL_0018:  ldarg.0
      IL_0019:  ldc.i4.m1
      IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_001f:  nop
      IL_0020:  ldarg.0
      IL_0021:  ldc.i4.0
      IL_0022:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__1'
      IL_0027:  br.s       IL_0068

      IL_0029:  nop
      IL_002a:  ldarg.0
      IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__1'
      IL_0030:  ldc.i4.2
      IL_0031:  rem
      IL_0032:  ldc.i4.0
      IL_0033:  ceq
      IL_0035:  stloc.1
      IL_0036:  ldloc.1
      IL_0037:  brfalse.s  IL_0057

      IL_0039:  nop
      IL_003a:  ldarg.0
      IL_003b:  ldarg.0
      IL_003c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__1'
      IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>2__current'
      IL_0046:  ldarg.0
      IL_0047:  ldc.i4.1
      IL_0048:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_004d:  ldc.i4.1
      IL_004e:  ret

      IL_004f:  ldarg.0
      IL_0050:  ldc.i4.m1
      IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<>1__state'
      IL_0056:  nop
      IL_0057:  nop
      IL_0058:  ldarg.0
      IL_0059:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__1'
      IL_005e:  stloc.2
      IL_005f:  ldarg.0
      IL_0060:  ldloc.2
      IL_0061:  ldc.i4.1
      IL_0062:  add
      IL_0063:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__1'
      IL_0068:  ldarg.0
      IL_0069:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::'<i>5__1'
      IL_006e:  ldarg.0
      IL_006f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__14'::n
      IL_0074:  clt
      IL_0076:  stloc.3
      IL_0077:  ldloc.3
      IL_0078:  brtrue.s   IL_0029

      IL_007a:  ldc.i4.0
      IL_007b:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<ExceptionHandling>d__15'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0014

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.3
      IL_0010:  beq.s      IL_0014

      IL_0012:  br.s       IL_0020

      IL_0014:  nop
      .try
      {
        IL_0015:  leave.s    IL_001e

      }  // end .try
      finally
      {
        IL_0017:  ldarg.0
        IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>m__Finally1'()
        IL_001d:  endfinally
      }  // end handler
      IL_001e:  br.s       IL_0020

      IL_0020:  ret
    } // end of method '<ExceptionHandling>d__15'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       230 (0xe6)
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
                              IL_001f,
                              IL_0021,
                              IL_0023,
                              IL_0025)
        IL_001d:  br.s       IL_002a

        IL_001f:  br.s       IL_0031

        IL_0021:  br.s       IL_004f

        IL_0023:  br.s       IL_0088

        IL_0025:  br         IL_00c8

        IL_002a:  ldc.i4.0
        IL_002b:  stloc.0
        IL_002c:  leave      IL_00e4

        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.m1
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_0038:  nop
        IL_0039:  ldarg.0
        IL_003a:  ldc.i4.s   97
        IL_003c:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.1
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_0048:  ldc.i4.1
        IL_0049:  stloc.0
        IL_004a:  leave      IL_00e4

        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.m1
        IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        .try
        {
          IL_0056:  nop
          IL_0057:  ldstr      "1 - try"
          IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0061:  nop
          IL_0062:  nop
          IL_0063:  leave.s    IL_0075

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_0065:  pop
          IL_0066:  nop
          IL_0067:  ldstr      "1 - catch"
          IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0071:  nop
          IL_0072:  nop
          IL_0073:  leave.s    IL_0075

        }  // end handler
        IL_0075:  ldarg.0
        IL_0076:  ldc.i4.s   98
        IL_0078:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
        IL_007d:  ldarg.0
        IL_007e:  ldc.i4.2
        IL_007f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_0084:  ldc.i4.1
        IL_0085:  stloc.0
        IL_0086:  leave.s    IL_00e4

        IL_0088:  ldarg.0
        IL_0089:  ldc.i4.m1
        IL_008a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_008f:  ldarg.0
        IL_0090:  ldc.i4.s   -3
        IL_0092:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_0097:  nop
        .try
        {
          IL_0098:  nop
          IL_0099:  ldstr      "2 - try"
          IL_009e:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00a3:  nop
          IL_00a4:  nop
          IL_00a5:  leave.s    IL_00b5

        }  // end .try
        finally
        {
          IL_00a7:  nop
          IL_00a8:  ldstr      "2 - finally"
          IL_00ad:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00b2:  nop
          IL_00b3:  nop
          IL_00b4:  endfinally
        }  // end handler
        IL_00b5:  ldarg.0
        IL_00b6:  ldc.i4.s   99
        IL_00b8:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>2__current'
        IL_00bd:  ldarg.0
        IL_00be:  ldc.i4.3
        IL_00bf:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_00c4:  ldc.i4.1
        IL_00c5:  stloc.0
        IL_00c6:  leave.s    IL_00e4

        IL_00c8:  ldarg.0
        IL_00c9:  ldc.i4.s   -3
        IL_00cb:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
        IL_00d0:  nop
        IL_00d1:  ldarg.0
        IL_00d2:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>m__Finally1'()
        IL_00d7:  nop
        IL_00d8:  ldc.i4.0
        IL_00d9:  stloc.0
        IL_00da:  leave.s    IL_00e4

      }  // end .try
      fault
      {
        IL_00dc:  ldarg.0
        IL_00dd:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::System.IDisposable.Dispose()
        IL_00e2:  nop
        IL_00e3:  endfinally
      }  // end handler
      IL_00e4:  ldloc.0
      IL_00e5:  ret
    } // end of method '<ExceptionHandling>d__15'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__15'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "outer finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       114 (0x72)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_003b

      IL_001f:  br.s       IL_0067

      IL_0021:  ldc.i4.0
      IL_0022:  ret

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.0
      IL_002d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>2__current'
      IL_0032:  ldarg.0
      IL_0033:  ldc.i4.1
      IL_0034:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0039:  ldc.i4.1
      IL_003a:  ret

      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.m1
      IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      .try
      {
        IL_0042:  nop
        IL_0043:  ldstr      "In Try"
        IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_004d:  nop
        IL_004e:  nop
        IL_004f:  leave.s    IL_0057

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0051:  pop
        IL_0052:  nop
        IL_0053:  ldc.i4.0
        IL_0054:  stloc.0
        IL_0055:  leave.s    IL_0070

      }  // end handler
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.1
      IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>2__current'
      IL_005e:  ldarg.0
      IL_005f:  ldc.i4.2
      IL_0060:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_0065:  ldc.i4.1
      IL_0066:  ret

      IL_0067:  ldarg.0
      IL_0068:  ldc.i4.m1
      IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__16'::'<>1__state'
      IL_006e:  ldc.i4.0
      IL_006f:  ret

      IL_0070:  ldloc.0
      IL_0071:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       35 (0x23)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0016

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  ldc.i4.1
      IL_0012:  ble.un.s   IL_0016

      IL_0014:  br.s       IL_0022

      IL_0016:  nop
      .try
      {
        IL_0017:  leave.s    IL_0020

      }  // end .try
      finally
      {
        IL_0019:  ldarg.0
        IL_001a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'()
        IL_001f:  endfinally
      }  // end handler
      IL_0020:  br.s       IL_0022

      IL_0022:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       160 (0xa0)
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
                              IL_001b,
                              IL_001d,
                              IL_001f)
        IL_0019:  br.s       IL_0021

        IL_001b:  br.s       IL_0025

        IL_001d:  br.s       IL_0048

        IL_001f:  br.s       IL_0077

        IL_0021:  ldc.i4.0
        IL_0022:  stloc.0
        IL_0023:  leave.s    IL_009e

        IL_0025:  ldarg.0
        IL_0026:  ldc.i4.m1
        IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_002c:  nop
        IL_002d:  ldarg.0
        IL_002e:  ldc.i4.s   -3
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0035:  nop
        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.0
        IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>2__current'
        IL_003d:  ldarg.0
        IL_003e:  ldc.i4.1
        IL_003f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0044:  ldc.i4.1
        IL_0045:  stloc.0
        IL_0046:  leave.s    IL_009e

        IL_0048:  ldarg.0
        IL_0049:  ldc.i4.s   -3
        IL_004b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        .try
        {
          IL_0050:  nop
          IL_0051:  ldstr      "In Try"
          IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005b:  nop
          IL_005c:  nop
          IL_005d:  leave.s    IL_0065

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_005f:  pop
          IL_0060:  nop
          IL_0061:  ldc.i4.0
          IL_0062:  stloc.0
          IL_0063:  leave.s    IL_0089

        }  // end handler
        IL_0065:  ldarg.0
        IL_0066:  ldc.i4.1
        IL_0067:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>2__current'
        IL_006c:  ldarg.0
        IL_006d:  ldc.i4.2
        IL_006e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_0073:  ldc.i4.1
        IL_0074:  stloc.0
        IL_0075:  leave.s    IL_009e

        IL_0077:  ldarg.0
        IL_0078:  ldc.i4.s   -3
        IL_007a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
        IL_007f:  nop
        IL_0080:  ldarg.0
        IL_0081:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'()
        IL_0086:  nop
        IL_0087:  br.s       IL_0092

        IL_0089:  ldarg.0
        IL_008a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>m__Finally1'()
        IL_008f:  nop
        IL_0090:  leave.s    IL_009e

        IL_0092:  ldc.i4.0
        IL_0093:  stloc.0
        IL_0094:  leave.s    IL_009e

      }  // end .try
      fault
      {
        IL_0096:  ldarg.0
        IL_0097:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::System.IDisposable.Dispose()
        IL_009c:  nop
        IL_009d:  endfinally
      }  // end handler
      IL_009e:  ldloc.0
      IL_009f:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__17'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__17'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       35 (0x23)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0016

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  ldc.i4.1
      IL_0012:  ble.un.s   IL_0016

      IL_0014:  br.s       IL_0022

      IL_0016:  nop
      .try
      {
        IL_0017:  leave.s    IL_0020

      }  // end .try
      finally
      {
        IL_0019:  ldarg.0
        IL_001a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'()
        IL_001f:  endfinally
      }  // end handler
      IL_0020:  br.s       IL_0022

      IL_0022:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       174 (0xae)
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
                              IL_001b,
                              IL_001d,
                              IL_001f)
        IL_0019:  br.s       IL_0021

        IL_001b:  br.s       IL_0028

        IL_001d:  br.s       IL_004b

        IL_001f:  br.s       IL_0085

        IL_0021:  ldc.i4.0
        IL_0022:  stloc.0
        IL_0023:  leave      IL_00ac

        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.m1
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_002f:  nop
        IL_0030:  ldarg.0
        IL_0031:  ldc.i4.s   -3
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_0038:  nop
        IL_0039:  ldarg.0
        IL_003a:  ldc.i4.0
        IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>2__current'
        IL_0040:  ldarg.0
        IL_0041:  ldc.i4.1
        IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_0047:  ldc.i4.1
        IL_0048:  stloc.0
        IL_0049:  leave.s    IL_00ac

        IL_004b:  ldarg.0
        IL_004c:  ldc.i4.s   -3
        IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        .try
        {
          IL_0053:  nop
          IL_0054:  ldstr      "In Try"
          IL_0059:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005e:  nop
          IL_005f:  ldc.i4.0
          IL_0060:  stloc.0
          IL_0061:  leave.s    IL_0097

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0063:  pop
          IL_0064:  nop
          IL_0065:  ldstr      "Catch"
          IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_006f:  nop
          IL_0070:  nop
          IL_0071:  leave.s    IL_0073

        }  // end handler
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.1
        IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>2__current'
        IL_007a:  ldarg.0
        IL_007b:  ldc.i4.2
        IL_007c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_0081:  ldc.i4.1
        IL_0082:  stloc.0
        IL_0083:  leave.s    IL_00ac

        IL_0085:  ldarg.0
        IL_0086:  ldc.i4.s   -3
        IL_0088:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
        IL_008d:  nop
        IL_008e:  ldarg.0
        IL_008f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'()
        IL_0094:  nop
        IL_0095:  br.s       IL_00a0

        IL_0097:  ldarg.0
        IL_0098:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>m__Finally1'()
        IL_009d:  nop
        IL_009e:  leave.s    IL_00ac

        IL_00a0:  ldc.i4.0
        IL_00a1:  stloc.0
        IL_00a2:  leave.s    IL_00ac

      }  // end .try
      fault
      {
        IL_00a4:  ldarg.0
        IL_00a5:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::System.IDisposable.Dispose()
        IL_00aa:  nop
        IL_00ab:  endfinally
      }  // end handler
      IL_00ac:  ldloc.0
      IL_00ad:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__18'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__18'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       35 (0x23)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0016

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  ldc.i4.1
      IL_0012:  ble.un.s   IL_0016

      IL_0014:  br.s       IL_0022

      IL_0016:  nop
      .try
      {
        IL_0017:  leave.s    IL_0020

      }  // end .try
      finally
      {
        IL_0019:  ldarg.0
        IL_001a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'()
        IL_001f:  endfinally
      }  // end handler
      IL_0020:  br.s       IL_0022

      IL_0022:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       186 (0xba)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001d,
                              IL_001f)
        IL_0019:  br.s       IL_0021

        IL_001b:  br.s       IL_0028

        IL_001d:  br.s       IL_004b

        IL_001f:  br.s       IL_0091

        IL_0021:  ldc.i4.0
        IL_0022:  stloc.0
        IL_0023:  leave      IL_00b8

        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.m1
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_002f:  nop
        IL_0030:  ldarg.0
        IL_0031:  ldc.i4.s   -3
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0038:  nop
        IL_0039:  ldarg.0
        IL_003a:  ldc.i4.0
        IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>2__current'
        IL_0040:  ldarg.0
        IL_0041:  ldc.i4.1
        IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0047:  ldc.i4.1
        IL_0048:  stloc.0
        IL_0049:  leave.s    IL_00b8

        IL_004b:  ldarg.0
        IL_004c:  ldc.i4.s   -3
        IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        .try
        {
          IL_0053:  nop
          IL_0054:  ldstr      "In Try"
          IL_0059:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005e:  nop
          IL_005f:  ldarg.0
          IL_0060:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::b
          IL_0065:  stloc.2
          IL_0066:  ldloc.2
          IL_0067:  brfalse.s  IL_006e

          IL_0069:  nop
          IL_006a:  ldc.i4.0
          IL_006b:  stloc.0
          IL_006c:  leave.s    IL_00a3

          IL_006e:  nop
          IL_006f:  leave.s    IL_007f

        }  // end .try
        finally
        {
          IL_0071:  nop
          IL_0072:  ldstr      "Inner Finally"
          IL_0077:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_007c:  nop
          IL_007d:  nop
          IL_007e:  endfinally
        }  // end handler
        IL_007f:  ldarg.0
        IL_0080:  ldc.i4.1
        IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>2__current'
        IL_0086:  ldarg.0
        IL_0087:  ldc.i4.2
        IL_0088:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_008d:  ldc.i4.1
        IL_008e:  stloc.0
        IL_008f:  leave.s    IL_00b8

        IL_0091:  ldarg.0
        IL_0092:  ldc.i4.s   -3
        IL_0094:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
        IL_0099:  nop
        IL_009a:  ldarg.0
        IL_009b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'()
        IL_00a0:  nop
        IL_00a1:  br.s       IL_00ac

        IL_00a3:  ldarg.0
        IL_00a4:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>m__Finally1'()
        IL_00a9:  nop
        IL_00aa:  leave.s    IL_00b8

        IL_00ac:  ldc.i4.0
        IL_00ad:  stloc.0
        IL_00ae:  leave.s    IL_00b8

      }  // end .try
      fault
      {
        IL_00b0:  ldarg.0
        IL_00b1:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::System.IDisposable.Dispose()
        IL_00b6:  nop
        IL_00b7:  endfinally
      }  // end handler
      IL_00b8:  ldloc.0
      IL_00b9:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__19'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__19'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       26 (0x1a)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_000e

      IL_000c:  br.s       IL_0010

      IL_000e:  ldc.i4.0
      IL_000f:  ret

      IL_0010:  ldarg.0
      IL_0011:  ldc.i4.m1
      IL_0012:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__20'::'<>1__state'
      IL_0017:  nop
      IL_0018:  ldc.i4.0
      IL_0019:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0014

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  beq.s      IL_0014

      IL_0012:  br.s       IL_0020

      IL_0014:  nop
      .try
      {
        IL_0015:  leave.s    IL_001e

      }  // end .try
      finally
      {
        IL_0017:  ldarg.0
        IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>m__Finally1'()
        IL_001d:  endfinally
      }  // end handler
      IL_001e:  br.s       IL_0020

      IL_0020:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::System.IDisposable.Dispose

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
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  brfalse.s  IL_0012

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.1
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0016

        IL_0012:  br.s       IL_001a

        IL_0014:  br.s       IL_003d

        IL_0016:  ldc.i4.0
        IL_0017:  stloc.0
        IL_0018:  leave.s    IL_0053

        IL_001a:  ldarg.0
        IL_001b:  ldc.i4.m1
        IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0021:  nop
        IL_0022:  ldarg.0
        IL_0023:  ldc.i4.s   -3
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_002a:  nop
        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.0
        IL_002d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>2__current'
        IL_0032:  ldarg.0
        IL_0033:  ldc.i4.1
        IL_0034:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0039:  ldc.i4.1
        IL_003a:  stloc.0
        IL_003b:  leave.s    IL_0053

        IL_003d:  ldarg.0
        IL_003e:  ldc.i4.s   -3
        IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
        IL_0045:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
        IL_004a:  throw

      }  // end .try
      fault
      {
        IL_004b:  ldarg.0
        IL_004c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::System.IDisposable.Dispose()
        IL_0051:  nop
        IL_0052:  endfinally
      }  // end handler
      IL_0053:  ldloc.0
      IL_0054:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__21'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__21'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       60 (0x3c)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -4
      IL_000a:  sub
      IL_000b:  ldc.i4.1
      IL_000c:  ble.un.s   IL_0016

      IL_000e:  br.s       IL_0010

      IL_0010:  ldloc.0
      IL_0011:  ldc.i4.1
      IL_0012:  beq.s      IL_0016

      IL_0014:  br.s       IL_003b

      IL_0016:  nop
      .try
      {
        IL_0017:  ldloc.0
        IL_0018:  ldc.i4.s   -4
        IL_001a:  beq.s      IL_0024

        IL_001c:  br.s       IL_001e

        IL_001e:  ldloc.0
        IL_001f:  ldc.i4.1
        IL_0020:  beq.s      IL_0024

        IL_0022:  br.s       IL_0030

        IL_0024:  nop
        .try
        {
          IL_0025:  leave.s    IL_002e

        }  // end .try
        finally
        {
          IL_0027:  ldarg.0
          IL_0028:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally2'()
          IL_002d:  endfinally
        }  // end handler
        IL_002e:  br.s       IL_0030

        IL_0030:  leave.s    IL_0039

      }  // end .try
      finally
      {
        IL_0032:  ldarg.0
        IL_0033:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally1'()
        IL_0038:  endfinally
      }  // end handler
      IL_0039:  br.s       IL_003b

      IL_003b:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       108 (0x6c)
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

        IL_000a:  br.s       IL_000c

        IL_000c:  ldloc.1
        IL_000d:  ldc.i4.1
        IL_000e:  beq.s      IL_0014

        IL_0010:  br.s       IL_0016

        IL_0012:  br.s       IL_001a

        IL_0014:  br.s       IL_0046

        IL_0016:  ldc.i4.0
        IL_0017:  stloc.0
        IL_0018:  leave.s    IL_006a

        IL_001a:  ldarg.0
        IL_001b:  ldc.i4.m1
        IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0021:  nop
        IL_0022:  ldarg.0
        IL_0023:  ldc.i4.s   -3
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_002a:  nop
        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.s   -4
        IL_002e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0033:  nop
        IL_0034:  ldarg.0
        IL_0035:  ldc.i4.0
        IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>2__current'
        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.1
        IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_0042:  ldc.i4.1
        IL_0043:  stloc.0
        IL_0044:  leave.s    IL_006a

        IL_0046:  ldarg.0
        IL_0047:  ldc.i4.s   -4
        IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally2'()
        IL_0055:  nop
        IL_0056:  nop
        IL_0057:  ldarg.0
        IL_0058:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally1'()
        IL_005d:  nop
        IL_005e:  ldc.i4.0
        IL_005f:  stloc.0
        IL_0060:  leave.s    IL_006a

      }  // end .try
      fault
      {
        IL_0062:  ldarg.0
        IL_0063:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::System.IDisposable.Dispose()
        IL_0068:  nop
        IL_0069:  endfinally
      }  // end handler
      IL_006a:  ldloc.0
      IL_006b:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldstr      "Outer Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__22'::'<>m__Finally1'

    .method private hidebysig instance void 
            '<>m__Finally2'() cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   -3
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__22'::'<>1__state'
      IL_0008:  nop
      IL_0009:  ldstr      "Inner Finally"
      IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0013:  nop
      IL_0014:  nop
      IL_0015:  ret
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
    .field private !T '<val>5__1'
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
      IL_0009:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>l__initialThreadId'
      IL_0019:  ret
    } // end of method '<LocalInFinally>d__23`1'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.s   -3
      IL_000a:  beq.s      IL_0014

      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ldc.i4.2
      IL_0010:  beq.s      IL_0014

      IL_0012:  br.s       IL_0020

      IL_0014:  nop
      .try
      {
        IL_0015:  leave.s    IL_001e

      }  // end .try
      finally
      {
        IL_0017:  ldarg.0
        IL_0018:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>m__Finally1'()
        IL_001d:  endfinally
      }  // end handler
      IL_001e:  br.s       IL_0020

      IL_0020:  ret
    } // end of method '<LocalInFinally>d__23`1'::System.IDisposable.Dispose

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       158 (0x9e)
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
                              IL_001f,
                              IL_0021,
                              IL_0023,
                              IL_0025)
        IL_001d:  br.s       IL_0027

        IL_001f:  br.s       IL_002b

        IL_0021:  br.s       IL_0045

        IL_0023:  br.s       IL_0067

        IL_0025:  br.s       IL_0089

        IL_0027:  ldc.i4.0
        IL_0028:  stloc.0
        IL_0029:  leave.s    IL_009c

        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.m1
        IL_002d:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0032:  nop
        IL_0033:  ldarg.0
        IL_0034:  ldc.i4.1
        IL_0035:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
        IL_003a:  ldarg.0
        IL_003b:  ldc.i4.1
        IL_003c:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0041:  ldc.i4.1
        IL_0042:  stloc.0
        IL_0043:  leave.s    IL_009c

        IL_0045:  ldarg.0
        IL_0046:  ldc.i4.m1
        IL_0047:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_004c:  ldarg.0
        IL_004d:  ldc.i4.s   -3
        IL_004f:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0054:  nop
        IL_0055:  ldarg.0
        IL_0056:  ldc.i4.2
        IL_0057:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
        IL_005c:  ldarg.0
        IL_005d:  ldc.i4.2
        IL_005e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0063:  ldc.i4.1
        IL_0064:  stloc.0
        IL_0065:  leave.s    IL_009c

        IL_0067:  ldarg.0
        IL_0068:  ldc.i4.s   -3
        IL_006a:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_006f:  nop
        IL_0070:  ldarg.0
        IL_0071:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>m__Finally1'()
        IL_0076:  nop
        IL_0077:  ldarg.0
        IL_0078:  ldc.i4.3
        IL_0079:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>2__current'
        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.3
        IL_0080:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0085:  ldc.i4.1
        IL_0086:  stloc.0
        IL_0087:  leave.s    IL_009c

        IL_0089:  ldarg.0
        IL_008a:  ldc.i4.m1
        IL_008b:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
        IL_0090:  ldc.i4.0
        IL_0091:  stloc.0
        IL_0092:  leave.s    IL_009c

      }  // end .try
      fault
      {
        IL_0094:  ldarg.0
        IL_0095:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::System.IDisposable.Dispose()
        IL_009a:  nop
        IL_009b:  endfinally
      }  // end handler
      IL_009c:  ldloc.0
      IL_009d:  ret
    } // end of method '<LocalInFinally>d__23`1'::MoveNext

    .method private hidebysig instance void 
            '<>m__Finally1'() cil managed
    {
      // Code size       70 (0x46)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<>1__state'
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.0
      IL_000a:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::a
      IL_000f:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<val>5__1'
      IL_0014:  ldarg.0
      IL_0015:  ldflda     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<val>5__1'
      IL_001a:  constrained. !T
      IL_0020:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0025:  nop
      IL_0026:  ldarg.0
      IL_0027:  ldflda     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<val>5__1'
      IL_002c:  constrained. !T
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  nop
      IL_0038:  nop
      IL_0039:  ldarg.0
      IL_003a:  ldflda     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__23`1'<!T>::'<val>5__1'
      IL_003f:  initobj    !T
      IL_0045:  ret
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
    .field private !T '<val>5__1'
    .field private int32 '<i>5__2'
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
      IL_0009:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       113 (0x71)
      .maxstack  3
      .locals init (int32 V_0,
               int32 V_1,
               bool V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  br.s       IL_000c

      IL_000c:  ldloc.0
      IL_000d:  ldc.i4.1
      IL_000e:  beq.s      IL_0014

      IL_0010:  br.s       IL_0016

      IL_0012:  br.s       IL_0018

      IL_0014:  br.s       IL_004a

      IL_0016:  ldc.i4.0
      IL_0017:  ret

      IL_0018:  ldarg.0
      IL_0019:  ldc.i4.m1
      IL_001a:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_001f:  nop
      IL_0020:  ldarg.0
      IL_0021:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
      IL_0026:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<val>5__1'
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.0
      IL_002d:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__2'
      IL_0032:  br.s       IL_0062

      IL_0034:  nop
      IL_0035:  ldarg.0
      IL_0036:  ldarg.0
      IL_0037:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<val>5__1'
      IL_003c:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>2__current'
      IL_0041:  ldarg.0
      IL_0042:  ldc.i4.1
      IL_0043:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0048:  ldc.i4.1
      IL_0049:  ret

      IL_004a:  ldarg.0
      IL_004b:  ldc.i4.m1
      IL_004c:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<>1__state'
      IL_0051:  nop
      IL_0052:  ldarg.0
      IL_0053:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__2'
      IL_0058:  stloc.1
      IL_0059:  ldarg.0
      IL_005a:  ldloc.1
      IL_005b:  ldc.i4.1
      IL_005c:  add
      IL_005d:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__2'
      IL_0062:  ldarg.0
      IL_0063:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__24`1'<!T>::'<i>5__2'
      IL_0068:  ldc.i4.3
      IL_0069:  clt
      IL_006b:  stloc.2
      IL_006c:  ldloc.2
      IL_006d:  brtrue.s   IL_0034

      IL_006f:  ldc.i4.0
      IL_0070:  ret
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
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<char> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__2'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  ret
  } // end of method YieldReturnPrettyTest::get_YieldChars

  .method assembly hidebysig static void 
          Print<T>(string name,
                   class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T> enumerator) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  3
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldstr      ": Test start"
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  br.s       IL_0032

    IL_0014:  nop
    IL_0015:  ldarg.0
    IL_0016:  ldstr      ": "
    IL_001b:  ldarg.1
    IL_001c:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!!T>::get_Current()
    IL_0021:  box        !!T
    IL_0026:  call       string [mscorlib]System.String::Concat(object,
                                                                object,
                                                                object)
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  ldarg.1
    IL_0033:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
    IL_0038:  stloc.0
    IL_0039:  ldloc.0
    IL_003a:  brtrue.s   IL_0014

    IL_003c:  ret
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
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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
      // Code size       26 (0x1a)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_000e:  ldarg.0
      IL_000f:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>l__initialThreadId'
      IL_0019:  ret
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
      // Code size       143 (0x8f)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001d,
                            IL_001f)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_0055

      IL_001f:  br.s       IL_0086

      IL_0021:  ldc.i4.0
      IL_0022:  ret

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0032:  ldarg.0
      IL_0033:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0038:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_003d:  stloc.1
      IL_003e:  ldloc.1
      IL_003f:  ldc.i4.1
      IL_0040:  add
      IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0046:  ldloc.1
      IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>2__current'
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.1
      IL_004e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0053:  ldc.i4.1
      IL_0054:  ret

      IL_0055:  ldarg.0
      IL_0056:  ldc.i4.m1
      IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_005c:  ldarg.0
      IL_005d:  ldarg.0
      IL_005e:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0063:  ldarg.0
      IL_0064:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>4__this'
      IL_0069:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_006e:  stloc.1
      IL_006f:  ldloc.1
      IL_0070:  ldc.i4.1
      IL_0071:  add
      IL_0072:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0077:  ldloc.1
      IL_0078:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>2__current'
      IL_007d:  ldarg.0
      IL_007e:  ldc.i4.2
      IL_007f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_0084:  ldc.i4.1
      IL_0085:  ret

      IL_0086:  ldarg.0
      IL_0087:  ldc.i4.m1
      IL_0088:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__1'::'<>1__state'
      IL_008d:  ldc.i4.0
      IL_008e:  ret
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
