



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly YieldReturn
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<char> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<get_YieldChars>d__0'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       134 (0x86)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0025,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_0044

      IL_0021:  br.s       IL_005e

      IL_0023:  br.s       IL_0078

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_0080

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.m1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldc.i4.s   97
      IL_0034:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0039:  ldarg.0
      IL_003a:  ldc.i4.1
      IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0040:  ldc.i4.1
      IL_0041:  stloc.0
      IL_0042:  br.s       IL_0084

      IL_0044:  ldarg.0
      IL_0045:  ldc.i4.m1
      IL_0046:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_004b:  ldarg.0
      IL_004c:  ldc.i4.s   98
      IL_004e:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0053:  ldarg.0
      IL_0054:  ldc.i4.2
      IL_0055:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_005a:  ldc.i4.1
      IL_005b:  stloc.0
      IL_005c:  br.s       IL_0084

      IL_005e:  ldarg.0
      IL_005f:  ldc.i4.m1
      IL_0060:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0065:  ldarg.0
      IL_0066:  ldc.i4.s   99
      IL_0068:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_006d:  ldarg.0
      IL_006e:  ldc.i4.3
      IL_006f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_0074:  ldc.i4.1
      IL_0075:  stloc.0
      IL_0076:  br.s       IL_0084

      IL_0078:  ldarg.0
      IL_0079:  ldc.i4.m1
      IL_007a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>1__state'
      IL_007f:  nop
      IL_0080:  ldc.i4.0
      IL_0081:  stloc.0
      IL_0082:  br.s       IL_0084

      IL_0084:  ldloc.0
      IL_0085:  ret
    } // end of method '<get_YieldChars>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance char  'System.Collections.Generic.IEnumerator<System.Char>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<char>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (char V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<get_YieldChars>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Char
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<SimpleYieldReturn>d__3'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       143 (0x8f)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0025,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_0047

      IL_0021:  br.s       IL_0064

      IL_0023:  br.s       IL_0081

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_0089

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.m1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldstr      "A"
      IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0043:  ldc.i4.1
      IL_0044:  stloc.0
      IL_0045:  br.s       IL_008d

      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.m1
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_004e:  ldarg.0
      IL_004f:  ldstr      "B"
      IL_0054:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0060:  ldc.i4.1
      IL_0061:  stloc.0
      IL_0062:  br.s       IL_008d

      IL_0064:  ldarg.0
      IL_0065:  ldc.i4.m1
      IL_0066:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_006b:  ldarg.0
      IL_006c:  ldstr      "C"
      IL_0071:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0076:  ldarg.0
      IL_0077:  ldc.i4.3
      IL_0078:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_007d:  ldc.i4.1
      IL_007e:  stloc.0
      IL_007f:  br.s       IL_008d

      IL_0081:  ldarg.0
      IL_0082:  ldc.i4.m1
      IL_0083:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>1__state'
      IL_0088:  nop
      IL_0089:  ldc.i4.0
      IL_008a:  stloc.0
      IL_008b:  br.s       IL_008d

      IL_008d:  ldloc.0
      IL_008e:  ret
    } // end of method '<SimpleYieldReturn>d__3'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<SimpleYieldReturn>d__3'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       143 (0x8f)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0025,
                            IL_001f,
                            IL_0021,
                            IL_0023)
      IL_001d:  br.s       IL_0027

      IL_001f:  br.s       IL_0047

      IL_0021:  br.s       IL_0064

      IL_0023:  br.s       IL_0081

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_0089

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.m1
      IL_002b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0030:  nop
      IL_0031:  ldarg.0
      IL_0032:  ldstr      "A"
      IL_0037:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.1
      IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0043:  ldc.i4.1
      IL_0044:  stloc.0
      IL_0045:  br.s       IL_008d

      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.m1
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_004e:  ldarg.0
      IL_004f:  ldstr      "B"
      IL_0054:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.2
      IL_005b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0060:  ldc.i4.1
      IL_0061:  stloc.0
      IL_0062:  br.s       IL_008d

      IL_0064:  ldarg.0
      IL_0065:  ldc.i4.m1
      IL_0066:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_006b:  ldarg.0
      IL_006c:  ldstr      "C"
      IL_0071:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0076:  ldarg.0
      IL_0077:  ldc.i4.3
      IL_0078:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_007d:  ldc.i4.1
      IL_007e:  stloc.0
      IL_007f:  br.s       IL_008d

      IL_0081:  ldarg.0
      IL_0082:  ldc.i4.m1
      IL_0083:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>1__state'
      IL_0088:  nop
      IL_0089:  ldc.i4.0
      IL_008a:  stloc.0
      IL_008b:  br.s       IL_008d

      IL_008d:  ldloc.0
      IL_008e:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<SimpleYieldReturnEnumerator>d__6'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       82 (0x52)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0040

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
      IL_003b:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
      IL_0040:  ldloc.0
      IL_0041:  ldarg.0
      IL_0042:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>3__p'
      IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::p
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  br.s       IL_0050

      IL_0050:  ldloc.1
      IL_0051:  ret
    } // end of method '<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnParameters>d__8'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       115 (0x73)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_001b,
                            IL_001d)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0042

      IL_001d:  br.s       IL_0065

      IL_001f:  br.s       IL_0023

      IL_0021:  br.s       IL_006d

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::p
      IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  stloc.0
      IL_0040:  br.s       IL_0071

      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.m1
      IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0049:  ldarg.0
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>4__this'
      IL_0050:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_0055:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.2
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_0061:  ldc.i4.1
      IL_0062:  stloc.0
      IL_0063:  br.s       IL_0071

      IL_0065:  ldarg.0
      IL_0066:  ldc.i4.m1
      IL_0067:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>1__state'
      IL_006c:  nop
      IL_006d:  ldc.i4.0
      IL_006e:  stloc.0
      IL_006f:  br.s       IL_0071

      IL_0071:  ldloc.0
      IL_0072:  ret
    } // end of method '<YieldReturnParameters>d__8'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<YieldReturnParameters>d__8'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       115 (0x73)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_001b,
                            IL_001d)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0042

      IL_001d:  br.s       IL_0065

      IL_001f:  br.s       IL_0023

      IL_0021:  br.s       IL_006d

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::p
      IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_0037:  ldarg.0
      IL_0038:  ldc.i4.1
      IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_003e:  ldc.i4.1
      IL_003f:  stloc.0
      IL_0040:  br.s       IL_0071

      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.m1
      IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0049:  ldarg.0
      IL_004a:  ldarg.0
      IL_004b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>4__this'
      IL_0050:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest::fieldOnThis
      IL_0055:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_005a:  ldarg.0
      IL_005b:  ldc.i4.2
      IL_005c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_0061:  ldc.i4.1
      IL_0062:  stloc.0
      IL_0063:  br.s       IL_0071

      IL_0065:  ldarg.0
      IL_0066:  ldc.i4.m1
      IL_0067:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>1__state'
      IL_006c:  nop
      IL_006d:  ldc.i4.0
      IL_006e:  stloc.0
      IL_006f:  br.s       IL_0071

      IL_0071:  ldloc.0
      IL_0072:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<YieldReturnParametersEnumerator>d__b'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnInLoop>d__d'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       113 (0x71)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0019,
                            IL_0017)
      IL_0015:  br.s       IL_001b

      IL_0017:  br.s       IL_0046

      IL_0019:  br.s       IL_001d

      IL_001b:  br.s       IL_006b

      IL_001d:  ldarg.0
      IL_001e:  ldc.i4.m1
      IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldc.i4.0
      IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_002c:  br.s       IL_005c

      IL_002e:  nop
      IL_002f:  ldarg.0
      IL_0030:  ldarg.0
      IL_0031:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>2__current'
      IL_003b:  ldarg.0
      IL_003c:  ldc.i4.1
      IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_0042:  ldc.i4.1
      IL_0043:  stloc.0
      IL_0044:  br.s       IL_006f

      IL_0046:  ldarg.0
      IL_0047:  ldc.i4.m1
      IL_0048:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>1__state'
      IL_004d:  nop
      IL_004e:  ldarg.0
      IL_004f:  dup
      IL_0050:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_0055:  ldc.i4.1
      IL_0056:  add
      IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_005c:  ldarg.0
      IL_005d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<i>5__e'
      IL_0062:  ldc.i4.s   100
      IL_0064:  clt
      IL_0066:  stloc.2
      IL_0067:  ldloc.2
      IL_0068:  brtrue.s   IL_002e

      IL_006a:  nop
      IL_006b:  ldc.i4.0
      IL_006c:  stloc.0
      IL_006d:  br.s       IL_006f

      IL_006f:  ldloc.0
      IL_0070:  ret
    } // end of method '<YieldReturnInLoop>d__d'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<YieldReturnInLoop>d__d'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::System.Collections.IEnumerable.GetEnumerator

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
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0029,
                              IL_0023,
                              IL_002b,
                              IL_0025,
                              IL_0027)
        IL_0021:  br.s       IL_002b

        IL_0023:  br.s       IL_0047

        IL_0025:  br.s       IL_0068

        IL_0027:  br.s       IL_0089

        IL_0029:  br.s       IL_002d

        IL_002b:  br.s       IL_0091

        IL_002d:  ldarg.0
        IL_002e:  ldc.i4.m1
        IL_002f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0034:  nop
        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.0
        IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.1
        IL_003e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0043:  ldc.i4.1
        IL_0044:  stloc.0
        IL_0045:  leave.s    IL_009d

        IL_0047:  ldarg.0
        IL_0048:  ldc.i4.m1
        IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.2
        IL_0051:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.1
        IL_0058:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.3
        IL_005f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0064:  ldc.i4.1
        IL_0065:  stloc.0
        IL_0066:  leave.s    IL_009d

        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.2
        IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_006f:  nop
        IL_0070:  ldarg.0
        IL_0071:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>m__Finally12'()
        IL_0076:  nop
        IL_0077:  ldarg.0
        IL_0078:  ldc.i4.2
        IL_0079:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.4
        IL_0080:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0085:  ldc.i4.1
        IL_0086:  stloc.0
        IL_0087:  leave.s    IL_009d

        IL_0089:  ldarg.0
        IL_008a:  ldc.i4.m1
        IL_008b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
        IL_0090:  nop
        IL_0091:  ldc.i4.0
        IL_0092:  stloc.0
        IL_0093:  leave.s    IL_009d

      }  // end .try
      fault
      {
        IL_0095:  ldarg.0
        IL_0096:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::System.IDisposable.Dispose()
        IL_009b:  nop
        IL_009c:  endfinally
      }  // end handler
      IL_009d:  nop
      IL_009e:  ldloc.0
      IL_009f:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.2
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001b,
                            IL_0019)
      IL_0017:  br.s       IL_001d

      IL_0019:  br.s       IL_001f

      IL_001b:  br.s       IL_001f

      IL_001d:  br.s       IL_002a

      .try
      {
        IL_001f:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0021:  ldarg.0
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>m__Finally12'()
        IL_0027:  nop
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  nop
      IL_002a:  ret
    } // end of method '<YieldReturnWithTryFinally>d__11'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::'<>1__state'
      IL_0008:  ldstr      "Finally!"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>3__breakInMiddle'
      IL_003b:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::breakInMiddle
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       497 (0x1f1)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_004e,
                              IL_0033,
                              IL_0050,
                              IL_0035,
                              IL_0050,
                              IL_003a,
                              IL_003f,
                              IL_0044,
                              IL_0049)
        IL_0031:  br.s       IL_0050

        IL_0033:  br.s       IL_0081

        IL_0035:  br         IL_00bf

        IL_003a:  br         IL_00fd

        IL_003f:  br         IL_0158

        IL_0044:  br         IL_0193

        IL_0049:  br         IL_01ce

        IL_004e:  br.s       IL_0055

        IL_0050:  br         IL_01e1

        IL_0055:  ldarg.0
        IL_0056:  ldc.i4.m1
        IL_0057:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_005c:  nop
        IL_005d:  ldstr      "Start of method - 1"
        IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0067:  nop
        IL_0068:  ldarg.0
        IL_0069:  ldstr      "Start of method"
        IL_006e:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.1
        IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_007a:  ldc.i4.1
        IL_007b:  stloc.0
        IL_007c:  leave      IL_01ee

        IL_0081:  ldarg.0
        IL_0082:  ldc.i4.m1
        IL_0083:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0088:  ldstr      "Start of method - 2"
        IL_008d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0092:  nop
        IL_0093:  nop
        IL_0094:  ldarg.0
        IL_0095:  ldc.i4.2
        IL_0096:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_009b:  ldstr      "Within outer try - 1"
        IL_00a0:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00a5:  nop
        IL_00a6:  ldarg.0
        IL_00a7:  ldstr      "Within outer try"
        IL_00ac:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_00b1:  ldarg.0
        IL_00b2:  ldc.i4.3
        IL_00b3:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00b8:  ldc.i4.1
        IL_00b9:  stloc.0
        IL_00ba:  leave      IL_01ee

        IL_00bf:  ldarg.0
        IL_00c0:  ldc.i4.2
        IL_00c1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00c6:  ldstr      "Within outer try - 2"
        IL_00cb:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00d0:  nop
        IL_00d1:  nop
        IL_00d2:  ldarg.0
        IL_00d3:  ldc.i4.4
        IL_00d4:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00d9:  ldstr      "Within inner try - 1"
        IL_00de:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00e3:  nop
        IL_00e4:  ldarg.0
        IL_00e5:  ldstr      "Within inner try"
        IL_00ea:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_00ef:  ldarg.0
        IL_00f0:  ldc.i4.5
        IL_00f1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_00f6:  ldc.i4.1
        IL_00f7:  stloc.0
        IL_00f8:  leave      IL_01ee

        IL_00fd:  ldarg.0
        IL_00fe:  ldc.i4.4
        IL_00ff:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0104:  ldstr      "Within inner try - 2"
        IL_0109:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_010e:  nop
        IL_010f:  ldarg.0
        IL_0110:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::breakInMiddle
        IL_0115:  ldc.i4.0
        IL_0116:  ceq
        IL_0118:  stloc.2
        IL_0119:  ldloc.2
        IL_011a:  brtrue.s   IL_0134

        IL_011c:  nop
        IL_011d:  ldstr      "Breaking..."
        IL_0122:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0127:  nop
        IL_0128:  ldarg.0
        IL_0129:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::System.IDisposable.Dispose()
        IL_012e:  nop
        IL_012f:  leave      IL_01e1

        IL_0134:  ldstr      "End of inner try - 1"
        IL_0139:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_013e:  nop
        IL_013f:  ldarg.0
        IL_0140:  ldstr      "End of inner try"
        IL_0145:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_014a:  ldarg.0
        IL_014b:  ldc.i4.6
        IL_014c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_0151:  ldc.i4.1
        IL_0152:  stloc.0
        IL_0153:  leave      IL_01ee

        IL_0158:  ldarg.0
        IL_0159:  ldc.i4.4
        IL_015a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_015f:  ldstr      "End of inner try - 2"
        IL_0164:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0169:  nop
        IL_016a:  nop
        IL_016b:  ldarg.0
        IL_016c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally17'()
        IL_0171:  nop
        IL_0172:  ldstr      "End of outer try - 1"
        IL_0177:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_017c:  nop
        IL_017d:  ldarg.0
        IL_017e:  ldstr      "End of outer try"
        IL_0183:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_0188:  ldarg.0
        IL_0189:  ldc.i4.7
        IL_018a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_018f:  ldc.i4.1
        IL_0190:  stloc.0
        IL_0191:  leave.s    IL_01ee

        IL_0193:  ldarg.0
        IL_0194:  ldc.i4.2
        IL_0195:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_019a:  ldstr      "End of outer try - 2"
        IL_019f:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01a4:  nop
        IL_01a5:  nop
        IL_01a6:  ldarg.0
        IL_01a7:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally16'()
        IL_01ac:  nop
        IL_01ad:  ldstr      "End of method - 1"
        IL_01b2:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01b7:  nop
        IL_01b8:  ldarg.0
        IL_01b9:  ldstr      "End of method"
        IL_01be:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
        IL_01c3:  ldarg.0
        IL_01c4:  ldc.i4.8
        IL_01c5:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_01ca:  ldc.i4.1
        IL_01cb:  stloc.0
        IL_01cc:  leave.s    IL_01ee

        IL_01ce:  ldarg.0
        IL_01cf:  ldc.i4.m1
        IL_01d0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_01d5:  ldstr      "End of method - 2"
        IL_01da:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_01df:  nop
        IL_01e0:  nop
        IL_01e1:  nop
        IL_01e2:  ldc.i4.0
        IL_01e3:  stloc.0
        IL_01e4:  leave.s    IL_01ee

      }  // end .try
      fault
      {
        IL_01e6:  ldarg.0
        IL_01e7:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::System.IDisposable.Dispose()
        IL_01ec:  nop
        IL_01ed:  endfinally
      }  // end handler
      IL_01ee:  nop
      IL_01ef:  ldloc.0
      IL_01f0:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       115 (0x73)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.2
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0033,
                            IL_0029,
                            IL_002f,
                            IL_002b,
                            IL_002d,
                            IL_0031)
      IL_0027:  br.s       IL_0035

      IL_0029:  br.s       IL_0037

      IL_002b:  br.s       IL_0037

      IL_002d:  br.s       IL_0037

      IL_002f:  br.s       IL_0037

      IL_0031:  br.s       IL_0037

      IL_0033:  br.s       IL_0037

      IL_0035:  br.s       IL_0072

      .try
      {
        IL_0037:  ldarg.0
        IL_0038:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
        IL_003d:  stloc.0
        IL_003e:  ldloc.0
        IL_003f:  ldc.i4.4
        IL_0040:  sub
        IL_0041:  switch     ( 
                              IL_0058,
                              IL_0054,
                              IL_0056)
        IL_0052:  br.s       IL_005a

        IL_0054:  br.s       IL_005c

        IL_0056:  br.s       IL_005c

        IL_0058:  br.s       IL_005c

        IL_005a:  br.s       IL_0067

        .try
        {
          IL_005c:  leave.s    IL_0066

        }  // end .try
        finally
        {
          IL_005e:  ldarg.0
          IL_005f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally17'()
          IL_0064:  nop
          IL_0065:  endfinally
        }  // end handler
        IL_0066:  nop
        IL_0067:  leave.s    IL_0071

      }  // end .try
      finally
      {
        IL_0069:  ldarg.0
        IL_006a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally16'()
        IL_006f:  nop
        IL_0070:  endfinally
      }  // end handler
      IL_0071:  nop
      IL_0072:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0008:  ldstr      "Outer Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
    } // end of method '<YieldReturnWithNestedTryFinally>d__15'::'<>m__Finally16'

    .method private hidebysig instance void 
            '<>m__Finally17'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.2
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>1__state'
      IL_0008:  ldstr      "Inner Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>3__input'
      IL_003b:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::input
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'System.Collections.Generic.IEnumerable<System.String>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       547 (0x223)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0067,
                              IL_0069,
                              IL_0069,
                              IL_003f,
                              IL_0044,
                              IL_0049,
                              IL_004e,
                              IL_0053,
                              IL_0058,
                              IL_005d,
                              IL_0069,
                              IL_0062)
        IL_003d:  br.s       IL_0069

        IL_003f:  br         IL_00c5

        IL_0044:  br         IL_0104

        IL_0049:  br         IL_0124

        IL_004e:  br         IL_0144

        IL_0053:  br         IL_0164

        IL_0058:  br         IL_0184

        IL_005d:  br         IL_01a2

        IL_0062:  br         IL_01f4

        IL_0067:  br.s       IL_006e

        IL_0069:  br         IL_0214

        IL_006e:  ldarg.0
        IL_006f:  ldc.i4.m1
        IL_0070:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_0075:  nop
        IL_0076:  nop
        IL_0077:  ldarg.0
        IL_0078:  ldarg.0
        IL_0079:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::input
        IL_007e:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0083:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
        IL_0088:  ldarg.0
        IL_0089:  ldc.i4.1
        IL_008a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_008f:  br.s       IL_00d5

        IL_0091:  ldarg.0
        IL_0092:  ldarg.0
        IL_0093:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
        IL_0098:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_009d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<line>5__1b'
        IL_00a2:  nop
        IL_00a3:  nop
        IL_00a4:  ldarg.0
        IL_00a5:  ldc.i4.2
        IL_00a6:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00ab:  ldarg.0
        IL_00ac:  ldarg.0
        IL_00ad:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<line>5__1b'
        IL_00b2:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_00b7:  ldarg.0
        IL_00b8:  ldc.i4.3
        IL_00b9:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00be:  ldc.i4.1
        IL_00bf:  stloc.0
        IL_00c0:  leave      IL_0220

        IL_00c5:  ldarg.0
        IL_00c6:  ldc.i4.2
        IL_00c7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00cc:  nop
        IL_00cd:  ldarg.0
        IL_00ce:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1f'()
        IL_00d3:  nop
        IL_00d4:  nop
        IL_00d5:  ldarg.0
        IL_00d6:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
        IL_00db:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00e0:  stloc.2
        IL_00e1:  ldloc.2
        IL_00e2:  brtrue.s   IL_0091

        IL_00e4:  ldarg.0
        IL_00e5:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1e'()
        IL_00ea:  nop
        IL_00eb:  ldarg.0
        IL_00ec:  ldstr      "A"
        IL_00f1:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_00f6:  ldarg.0
        IL_00f7:  ldc.i4.4
        IL_00f8:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_00fd:  ldc.i4.1
        IL_00fe:  stloc.0
        IL_00ff:  leave      IL_0220

        IL_0104:  ldarg.0
        IL_0105:  ldc.i4.m1
        IL_0106:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_010b:  ldarg.0
        IL_010c:  ldstr      "B"
        IL_0111:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0116:  ldarg.0
        IL_0117:  ldc.i4.5
        IL_0118:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_011d:  ldc.i4.1
        IL_011e:  stloc.0
        IL_011f:  leave      IL_0220

        IL_0124:  ldarg.0
        IL_0125:  ldc.i4.m1
        IL_0126:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_012b:  ldarg.0
        IL_012c:  ldstr      "C"
        IL_0131:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0136:  ldarg.0
        IL_0137:  ldc.i4.6
        IL_0138:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_013d:  ldc.i4.1
        IL_013e:  stloc.0
        IL_013f:  leave      IL_0220

        IL_0144:  ldarg.0
        IL_0145:  ldc.i4.m1
        IL_0146:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_014b:  ldarg.0
        IL_014c:  ldstr      "D"
        IL_0151:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0156:  ldarg.0
        IL_0157:  ldc.i4.7
        IL_0158:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_015d:  ldc.i4.1
        IL_015e:  stloc.0
        IL_015f:  leave      IL_0220

        IL_0164:  ldarg.0
        IL_0165:  ldc.i4.m1
        IL_0166:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_016b:  ldarg.0
        IL_016c:  ldstr      "E"
        IL_0171:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0176:  ldarg.0
        IL_0177:  ldc.i4.8
        IL_0178:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_017d:  ldc.i4.1
        IL_017e:  stloc.0
        IL_017f:  leave      IL_0220

        IL_0184:  ldarg.0
        IL_0185:  ldc.i4.m1
        IL_0186:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_018b:  ldarg.0
        IL_018c:  ldstr      "F"
        IL_0191:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_0196:  ldarg.0
        IL_0197:  ldc.i4.s   9
        IL_0199:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_019e:  ldc.i4.1
        IL_019f:  stloc.0
        IL_01a0:  leave.s    IL_0220

        IL_01a2:  ldarg.0
        IL_01a3:  ldc.i4.m1
        IL_01a4:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_01a9:  nop
        IL_01aa:  ldarg.0
        IL_01ab:  ldarg.0
        IL_01ac:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::input
        IL_01b1:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_01b6:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
        IL_01bb:  ldarg.0
        IL_01bc:  ldc.i4.s   10
        IL_01be:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_01c3:  br.s       IL_01fd

        IL_01c5:  ldarg.0
        IL_01c6:  ldarg.0
        IL_01c7:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
        IL_01cc:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_01d1:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<item>5__1c'
        IL_01d6:  nop
        IL_01d7:  ldarg.0
        IL_01d8:  ldarg.0
        IL_01d9:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<item>5__1c'
        IL_01de:  callvirt   instance string [mscorlib]System.String::ToUpper()
        IL_01e3:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
        IL_01e8:  ldarg.0
        IL_01e9:  ldc.i4.s   11
        IL_01eb:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_01f0:  ldc.i4.1
        IL_01f1:  stloc.0
        IL_01f2:  leave.s    IL_0220

        IL_01f4:  ldarg.0
        IL_01f5:  ldc.i4.s   10
        IL_01f7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_01fc:  nop
        IL_01fd:  ldarg.0
        IL_01fe:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
        IL_0203:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0208:  stloc.2
        IL_0209:  ldloc.2
        IL_020a:  brtrue.s   IL_01c5

        IL_020c:  ldarg.0
        IL_020d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally21'()
        IL_0212:  nop
        IL_0213:  nop
        IL_0214:  ldc.i4.0
        IL_0215:  stloc.0
        IL_0216:  leave.s    IL_0220

      }  // end .try
      fault
      {
        IL_0218:  ldarg.0
        IL_0219:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.IDisposable.Dispose()
        IL_021e:  nop
        IL_021f:  endfinally
      }  // end handler
      IL_0220:  nop
      IL_0221:  ldloc.0
      IL_0222:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance string  'System.Collections.Generic.IEnumerator<System.String>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       134 (0x86)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0021,
                            IL_001f,
                            IL_001d)
      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0025

      IL_0021:  br.s       IL_0025

      IL_0023:  br.s       IL_005a

      .try
      {
        IL_0025:  ldarg.0
        IL_0026:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
        IL_002b:  stloc.0
        IL_002c:  ldloc.0
        IL_002d:  ldc.i4.2
        IL_002e:  sub
        IL_002f:  switch     ( 
                              IL_0040,
                              IL_003e)
        IL_003c:  br.s       IL_0042

        IL_003e:  br.s       IL_0044

        IL_0040:  br.s       IL_0044

        IL_0042:  br.s       IL_004f

        .try
        {
          IL_0044:  leave.s    IL_004e

        }  // end .try
        finally
        {
          IL_0046:  ldarg.0
          IL_0047:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1f'()
          IL_004c:  nop
          IL_004d:  endfinally
        }  // end handler
        IL_004e:  nop
        IL_004f:  leave.s    IL_0059

      }  // end .try
      finally
      {
        IL_0051:  ldarg.0
        IL_0052:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1e'()
        IL_0057:  nop
        IL_0058:  endfinally
      }  // end handler
      IL_0059:  nop
      IL_005a:  ldarg.0
      IL_005b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0060:  stloc.0
      IL_0061:  ldloc.0
      IL_0062:  ldc.i4.s   10
      IL_0064:  sub
      IL_0065:  switch     ( 
                            IL_0076,
                            IL_0074)
      IL_0072:  br.s       IL_0078

      IL_0074:  br.s       IL_007a

      IL_0076:  br.s       IL_007a

      IL_0078:  br.s       IL_0085

      .try
      {
        IL_007a:  leave.s    IL_0084

      }  // end .try
      finally
      {
        IL_007c:  ldarg.0
        IL_007d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally21'()
        IL_0082:  nop
        IL_0083:  endfinally
      }  // end handler
      IL_0084:  nop
      IL_0085:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
      IL_000d:  ldnull
      IL_000e:  ceq
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brtrue.s   IL_0020

      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap1d'
      IL_001a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001f:  nop
      IL_0020:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1e'

    .method private hidebysig instance void 
            '<>m__Finally1f'() cil managed
    {
      // Code size       32 (0x20)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0008:  ldstr      "Processed "
      IL_000d:  ldarg.0
      IL_000e:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<line>5__1b'
      IL_0013:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  ret
    } // end of method '<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>m__Finally1f'

    .method private hidebysig instance void 
            '<>m__Finally21'() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
      IL_000d:  ldnull
      IL_000e:  ceq
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brtrue.s   IL_0020

      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>7__wrap20'
      IL_001a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001f:  nop
      IL_0020:  ret
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
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::line
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>3__input'
      IL_003b:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::input
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       217 (0xd9)
      .maxstack  4
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0020,
                              IL_0022,
                              IL_001b)
        IL_0019:  br.s       IL_0022

        IL_001b:  br         IL_00ab

        IL_0020:  br.s       IL_0027

        IL_0022:  br         IL_00ca

        IL_0027:  ldarg.0
        IL_0028:  ldc.i4.m1
        IL_0029:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_002e:  nop
        IL_002f:  nop
        IL_0030:  ldarg.0
        IL_0031:  ldarg.0
        IL_0032:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::input
        IL_0037:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_003c:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.1
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_0048:  br.s       IL_00b3

        IL_004a:  ldarg.0
        IL_004b:  ldnull
        IL_004c:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_0051:  ldarg.0
        IL_0052:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::.ctor()
        IL_0057:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>8__locals27'
        IL_005c:  ldarg.0
        IL_005d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>8__locals27'
        IL_0062:  ldarg.0
        IL_0063:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
        IL_0068:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_006d:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::line
        IL_0072:  nop
        IL_0073:  ldarg.0
        IL_0074:  ldarg.0
        IL_0075:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_007a:  brtrue.s   IL_0095

        IL_007c:  ldarg.0
        IL_007d:  ldarg.0
        IL_007e:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>8__locals27'
        IL_0083:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass26'::'<YieldReturnWithAnonymousMethods1>b__24'()
        IL_0089:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_008e:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_0093:  br.s       IL_0095

        IL_0095:  ldarg.0
        IL_0096:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'CS$<>9__CachedAnonymousMethodDelegate25'
        IL_009b:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>2__current'
        IL_00a0:  ldarg.0
        IL_00a1:  ldc.i4.2
        IL_00a2:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_00a7:  ldc.i4.1
        IL_00a8:  stloc.0
        IL_00a9:  leave.s    IL_00d6

        IL_00ab:  ldarg.0
        IL_00ac:  ldc.i4.1
        IL_00ad:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
        IL_00b2:  nop
        IL_00b3:  ldarg.0
        IL_00b4:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
        IL_00b9:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00be:  stloc.2
        IL_00bf:  ldloc.2
        IL_00c0:  brtrue.s   IL_004a

        IL_00c2:  ldarg.0
        IL_00c3:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>m__Finally2a'()
        IL_00c8:  nop
        IL_00c9:  nop
        IL_00ca:  ldc.i4.0
        IL_00cb:  stloc.0
        IL_00cc:  leave.s    IL_00d6

      }  // end .try
      fault
      {
        IL_00ce:  ldarg.0
        IL_00cf:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::System.IDisposable.Dispose()
        IL_00d4:  nop
        IL_00d5:  endfinally
      }  // end handler
      IL_00d6:  nop
      IL_00d7:  ldloc.0
      IL_00d8:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance class [mscorlib]System.Func`1<string> 
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class [mscorlib]System.Func`1<string> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001b,
                            IL_0019)
      IL_0017:  br.s       IL_001d

      IL_0019:  br.s       IL_001f

      IL_001b:  br.s       IL_001f

      IL_001d:  br.s       IL_002a

      .try
      {
        IL_001f:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0021:  ldarg.0
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>m__Finally2a'()
        IL_0027:  nop
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  nop
      IL_002a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods1>d__28'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
      IL_000d:  ldnull
      IL_000e:  ceq
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brtrue.s   IL_0020

      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>7__wrap29'
      IL_001a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001f:  nop
      IL_0020:  ret
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
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::copy
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>3__input'
      IL_003b:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::input
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'System.Collections.Generic.IEnumerable<System.Func<System.String>>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       197 (0xc5)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_001d,
                              IL_001f,
                              IL_001b)
        IL_0019:  br.s       IL_001f

        IL_001b:  br.s       IL_0097

        IL_001d:  br.s       IL_0024

        IL_001f:  br         IL_00b6

        IL_0024:  ldarg.0
        IL_0025:  ldc.i4.m1
        IL_0026:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_002b:  nop
        IL_002c:  nop
        IL_002d:  ldarg.0
        IL_002e:  ldarg.0
        IL_002f:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::input
        IL_0034:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
        IL_0039:  stfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
        IL_003e:  ldarg.0
        IL_003f:  ldc.i4.1
        IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_0045:  br.s       IL_009f

        IL_0047:  ldarg.0
        IL_0048:  ldarg.0
        IL_0049:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
        IL_004e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<string>::get_Current()
        IL_0053:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<item>5__31'
        IL_0058:  ldarg.0
        IL_0059:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::.ctor()
        IL_005e:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'CS$<>8__locals2f'
        IL_0063:  nop
        IL_0064:  ldarg.0
        IL_0065:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'CS$<>8__locals2f'
        IL_006a:  ldarg.0
        IL_006b:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<item>5__31'
        IL_0070:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::copy
        IL_0075:  ldarg.0
        IL_0076:  ldarg.0
        IL_0077:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e' ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'CS$<>8__locals2f'
        IL_007c:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<>c__DisplayClass2e'::'<YieldReturnWithAnonymousMethods2>b__2d'()
        IL_0082:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                        native int)
        IL_0087:  stfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>2__current'
        IL_008c:  ldarg.0
        IL_008d:  ldc.i4.2
        IL_008e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_0093:  ldc.i4.1
        IL_0094:  stloc.0
        IL_0095:  leave.s    IL_00c2

        IL_0097:  ldarg.0
        IL_0098:  ldc.i4.1
        IL_0099:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
        IL_009e:  nop
        IL_009f:  ldarg.0
        IL_00a0:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
        IL_00a5:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_00aa:  stloc.2
        IL_00ab:  ldloc.2
        IL_00ac:  brtrue.s   IL_0047

        IL_00ae:  ldarg.0
        IL_00af:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>m__Finally33'()
        IL_00b4:  nop
        IL_00b5:  nop
        IL_00b6:  ldc.i4.0
        IL_00b7:  stloc.0
        IL_00b8:  leave.s    IL_00c2

      }  // end .try
      fault
      {
        IL_00ba:  ldarg.0
        IL_00bb:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::System.IDisposable.Dispose()
        IL_00c0:  nop
        IL_00c1:  endfinally
      }  // end handler
      IL_00c2:  nop
      IL_00c3:  ldloc.0
      IL_00c4:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance class [mscorlib]System.Func`1<string> 
            'System.Collections.Generic.IEnumerator<System.Func<System.String>>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<class [mscorlib]System.Func`1<string>>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class [mscorlib]System.Func`1<string> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001b,
                            IL_0019)
      IL_0017:  br.s       IL_001d

      IL_0019:  br.s       IL_001f

      IL_001b:  br.s       IL_001f

      IL_001d:  br.s       IL_002a

      .try
      {
        IL_001f:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0021:  ldarg.0
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>m__Finally33'()
        IL_0027:  nop
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  nop
      IL_002a:  ret
    } // end of method '<YieldReturnWithAnonymousMethods2>d__30'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Func`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.m1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>1__state'
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
      IL_000d:  ldnull
      IL_000e:  ceq
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  brtrue.s   IL_0020

      IL_0014:  ldarg.0
      IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerator`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>7__wrap32'
      IL_001a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001f:  nop
      IL_0020:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>3__n'
      IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::n
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<GetEvenNumbers>d__36'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       137 (0x89)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0019,
                            IL_0017)
      IL_0015:  br.s       IL_001b

      IL_0017:  br.s       IL_0059

      IL_0019:  br.s       IL_001d

      IL_001b:  br.s       IL_0083

      IL_001d:  ldarg.0
      IL_001e:  ldc.i4.m1
      IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldc.i4.0
      IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_002c:  br.s       IL_0070

      IL_002e:  nop
      IL_002f:  ldarg.0
      IL_0030:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0035:  ldc.i4.2
      IL_0036:  rem
      IL_0037:  ldc.i4.0
      IL_0038:  ceq
      IL_003a:  ldc.i4.0
      IL_003b:  ceq
      IL_003d:  stloc.2
      IL_003e:  ldloc.2
      IL_003f:  brtrue.s   IL_0061

      IL_0041:  nop
      IL_0042:  ldarg.0
      IL_0043:  ldarg.0
      IL_0044:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>2__current'
      IL_004e:  ldarg.0
      IL_004f:  ldc.i4.1
      IL_0050:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0055:  ldc.i4.1
      IL_0056:  stloc.0
      IL_0057:  br.s       IL_0087

      IL_0059:  ldarg.0
      IL_005a:  ldc.i4.m1
      IL_005b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>1__state'
      IL_0060:  nop
      IL_0061:  nop
      IL_0062:  ldarg.0
      IL_0063:  dup
      IL_0064:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0069:  ldc.i4.1
      IL_006a:  add
      IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0070:  ldarg.0
      IL_0071:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<i>5__37'
      IL_0076:  ldarg.0
      IL_0077:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::n
      IL_007c:  clt
      IL_007e:  stloc.2
      IL_007f:  ldloc.2
      IL_0080:  brtrue.s   IL_002e

      IL_0082:  nop
      IL_0083:  ldc.i4.0
      IL_0084:  stloc.0
      IL_0085:  br.s       IL_0087

      IL_0087:  ldloc.0
      IL_0088:  ret
    } // end of method '<GetEvenNumbers>d__36'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<GetEvenNumbers>d__36'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<char> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<char> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'System.Collections.Generic.IEnumerable<System.Char>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<ExceptionHandling>d__3a'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       242 (0xf2)
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
                              IL_0034,
                              IL_002b,
                              IL_0036,
                              IL_002d,
                              IL_0036,
                              IL_0036,
                              IL_002f)
        IL_0029:  br.s       IL_0036

        IL_002b:  br.s       IL_0059

        IL_002d:  br.s       IL_0093

        IL_002f:  br         IL_00d3

        IL_0034:  br.s       IL_003b

        IL_0036:  br         IL_00e3

        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.m1
        IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_0042:  nop
        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.s   97
        IL_0046:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
        IL_004b:  ldarg.0
        IL_004c:  ldc.i4.1
        IL_004d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_0052:  ldc.i4.1
        IL_0053:  stloc.0
        IL_0054:  leave      IL_00ef

        IL_0059:  ldarg.0
        IL_005a:  ldc.i4.m1
        IL_005b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        .try
        {
          IL_0060:  nop
          IL_0061:  ldstr      "1 - try"
          IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_006b:  nop
          IL_006c:  nop
          IL_006d:  leave.s    IL_007f

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_006f:  pop
          IL_0070:  nop
          IL_0071:  ldstr      "1 - catch"
          IL_0076:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_007b:  nop
          IL_007c:  nop
          IL_007d:  leave.s    IL_007f

        }  // end handler
        IL_007f:  nop
        IL_0080:  ldarg.0
        IL_0081:  ldc.i4.s   98
        IL_0083:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
        IL_0088:  ldarg.0
        IL_0089:  ldc.i4.3
        IL_008a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_008f:  ldc.i4.1
        IL_0090:  stloc.0
        IL_0091:  leave.s    IL_00ef

        IL_0093:  ldarg.0
        IL_0094:  ldc.i4.m1
        IL_0095:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_009a:  nop
        IL_009b:  ldarg.0
        IL_009c:  ldc.i4.4
        IL_009d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        .try
        {
          IL_00a2:  nop
          IL_00a3:  ldstr      "2 - try"
          IL_00a8:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00ad:  nop
          IL_00ae:  nop
          IL_00af:  leave.s    IL_00bf

        }  // end .try
        finally
        {
          IL_00b1:  nop
          IL_00b2:  ldstr      "2 - finally"
          IL_00b7:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00bc:  nop
          IL_00bd:  nop
          IL_00be:  endfinally
        }  // end handler
        IL_00bf:  nop
        IL_00c0:  ldarg.0
        IL_00c1:  ldc.i4.s   99
        IL_00c3:  stfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
        IL_00c8:  ldarg.0
        IL_00c9:  ldc.i4.6
        IL_00ca:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_00cf:  ldc.i4.1
        IL_00d0:  stloc.0
        IL_00d1:  leave.s    IL_00ef

        IL_00d3:  ldarg.0
        IL_00d4:  ldc.i4.4
        IL_00d5:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
        IL_00da:  nop
        IL_00db:  ldarg.0
        IL_00dc:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>m__Finally3b'()
        IL_00e1:  nop
        IL_00e2:  nop
        IL_00e3:  ldc.i4.0
        IL_00e4:  stloc.0
        IL_00e5:  leave.s    IL_00ef

      }  // end .try
      fault
      {
        IL_00e7:  ldarg.0
        IL_00e8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::System.IDisposable.Dispose()
        IL_00ed:  nop
        IL_00ee:  endfinally
      }  // end handler
      IL_00ef:  nop
      IL_00f0:  ldloc.0
      IL_00f1:  ret
    } // end of method '<ExceptionHandling>d__3a'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance char  'System.Collections.Generic.IEnumerator<System.Char>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<char>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (char V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       47 (0x2f)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.4
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001f,
                            IL_0021,
                            IL_001d)
      IL_001b:  br.s       IL_0021

      IL_001d:  br.s       IL_0023

      IL_001f:  br.s       IL_0023

      IL_0021:  br.s       IL_002e

      .try
      {
        IL_0023:  leave.s    IL_002d

      }  // end .try
      finally
      {
        IL_0025:  ldarg.0
        IL_0026:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>m__Finally3b'()
        IL_002b:  nop
        IL_002c:  endfinally
      }  // end handler
      IL_002d:  nop
      IL_002e:  ret
    } // end of method '<ExceptionHandling>d__3a'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      char ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Char
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::'<>1__state'
      IL_0008:  ldstr      "outer finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       125 (0x7d)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0023,
                            IL_001f,
                            IL_0025,
                            IL_0021)
      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0041

      IL_0021:  br.s       IL_006e

      IL_0023:  br.s       IL_0027

      IL_0025:  br.s       IL_0076

      IL_0027:  ldarg.0
      IL_0028:  ldc.i4.m1
      IL_0029:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_002e:  nop
      IL_002f:  ldarg.0
      IL_0030:  ldc.i4.0
      IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0036:  ldarg.0
      IL_0037:  ldc.i4.1
      IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_003d:  ldc.i4.1
      IL_003e:  stloc.0
      IL_003f:  br.s       IL_007b

      IL_0041:  ldarg.0
      IL_0042:  ldc.i4.m1
      IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      .try
      {
        IL_0048:  nop
        IL_0049:  ldstr      "In Try"
        IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0053:  nop
        IL_0054:  nop
        IL_0055:  leave.s    IL_005b

      }  // end .try
      catch [mscorlib]System.Object 
      {
        IL_0057:  pop
        IL_0058:  nop
        IL_0059:  leave.s    IL_0076

      }  // end handler
      IL_005b:  nop
      IL_005c:  ldarg.0
      IL_005d:  ldc.i4.1
      IL_005e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0063:  ldarg.0
      IL_0064:  ldc.i4.3
      IL_0065:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_006a:  ldc.i4.1
      IL_006b:  stloc.0
      IL_006c:  br.s       IL_007b

      IL_006e:  ldarg.0
      IL_006f:  ldc.i4.m1
      IL_0070:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>1__state'
      IL_0075:  nop
      IL_0076:  nop
      IL_0077:  ldc.i4.0
      IL_0078:  stloc.0
      IL_0079:  br.s       IL_007b

      IL_007b:  ldloc.0
      IL_007c:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<YieldBreakInCatch>d__3e'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       161 (0xa1)
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
                              IL_0027,
                              IL_0029,
                              IL_0023,
                              IL_0029,
                              IL_0025)
        IL_0021:  br.s       IL_0029

        IL_0023:  br.s       IL_004d

        IL_0025:  br.s       IL_0081

        IL_0027:  br.s       IL_002b

        IL_0029:  br.s       IL_0091

        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.m1
        IL_002d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_0032:  nop
        IL_0033:  nop
        IL_0034:  ldarg.0
        IL_0035:  ldc.i4.1
        IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.0
        IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.2
        IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_0049:  ldc.i4.1
        IL_004a:  stloc.0
        IL_004b:  leave.s    IL_009e

        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.1
        IL_004f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        .try
        {
          IL_0054:  nop
          IL_0055:  ldstr      "In Try"
          IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005f:  nop
          IL_0060:  nop
          IL_0061:  leave.s    IL_006e

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0063:  pop
          IL_0064:  nop
          IL_0065:  ldarg.0
          IL_0066:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::System.IDisposable.Dispose()
          IL_006b:  nop
          IL_006c:  leave.s    IL_0091

        }  // end handler
        IL_006e:  nop
        IL_006f:  ldarg.0
        IL_0070:  ldc.i4.1
        IL_0071:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
        IL_0076:  ldarg.0
        IL_0077:  ldc.i4.4
        IL_0078:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_007d:  ldc.i4.1
        IL_007e:  stloc.0
        IL_007f:  leave.s    IL_009e

        IL_0081:  ldarg.0
        IL_0082:  ldc.i4.1
        IL_0083:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
        IL_0088:  nop
        IL_0089:  ldarg.0
        IL_008a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>m__Finally42'()
        IL_008f:  nop
        IL_0090:  nop
        IL_0091:  nop
        IL_0092:  ldc.i4.0
        IL_0093:  stloc.0
        IL_0094:  leave.s    IL_009e

      }  // end .try
      fault
      {
        IL_0096:  ldarg.0
        IL_0097:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::System.IDisposable.Dispose()
        IL_009c:  nop
        IL_009d:  endfinally
      }  // end handler
      IL_009e:  nop
      IL_009f:  ldloc.0
      IL_00a0:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       53 (0x35)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0025,
                            IL_0021,
                            IL_0027,
                            IL_0023)
      IL_001f:  br.s       IL_0027

      IL_0021:  br.s       IL_0029

      IL_0023:  br.s       IL_0029

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_0034

      .try
      {
        IL_0029:  leave.s    IL_0033

      }  // end .try
      finally
      {
        IL_002b:  ldarg.0
        IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>m__Finally42'()
        IL_0031:  nop
        IL_0032:  endfinally
      }  // end handler
      IL_0033:  nop
      IL_0034:  ret
    } // end of method '<YieldBreakInCatchInTryFinally>d__41'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::'<>1__state'
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       172 (0xac)
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
                              IL_0027,
                              IL_0029,
                              IL_0023,
                              IL_0029,
                              IL_0025)
        IL_0021:  br.s       IL_0029

        IL_0023:  br.s       IL_004d

        IL_0025:  br.s       IL_008c

        IL_0027:  br.s       IL_002b

        IL_0029:  br.s       IL_009c

        IL_002b:  ldarg.0
        IL_002c:  ldc.i4.m1
        IL_002d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0032:  nop
        IL_0033:  nop
        IL_0034:  ldarg.0
        IL_0035:  ldc.i4.1
        IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_003b:  ldarg.0
        IL_003c:  ldc.i4.0
        IL_003d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.2
        IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0049:  ldc.i4.1
        IL_004a:  stloc.0
        IL_004b:  leave.s    IL_00a9

        IL_004d:  ldarg.0
        IL_004e:  ldc.i4.1
        IL_004f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        .try
        {
          IL_0054:  nop
          IL_0055:  ldstr      "In Try"
          IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_005f:  nop
          IL_0060:  ldarg.0
          IL_0061:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::System.IDisposable.Dispose()
          IL_0066:  nop
          IL_0067:  leave.s    IL_009c

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0069:  pop
          IL_006a:  nop
          IL_006b:  ldstr      "Catch"
          IL_0070:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0075:  nop
          IL_0076:  nop
          IL_0077:  leave.s    IL_0079

        }  // end handler
        IL_0079:  nop
        IL_007a:  ldarg.0
        IL_007b:  ldc.i4.1
        IL_007c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
        IL_0081:  ldarg.0
        IL_0082:  ldc.i4.4
        IL_0083:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0088:  ldc.i4.1
        IL_0089:  stloc.0
        IL_008a:  leave.s    IL_00a9

        IL_008c:  ldarg.0
        IL_008d:  ldc.i4.1
        IL_008e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
        IL_0093:  nop
        IL_0094:  ldarg.0
        IL_0095:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>m__Finally46'()
        IL_009a:  nop
        IL_009b:  nop
        IL_009c:  nop
        IL_009d:  ldc.i4.0
        IL_009e:  stloc.0
        IL_009f:  leave.s    IL_00a9

      }  // end .try
      fault
      {
        IL_00a1:  ldarg.0
        IL_00a2:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::System.IDisposable.Dispose()
        IL_00a7:  nop
        IL_00a8:  endfinally
      }  // end handler
      IL_00a9:  nop
      IL_00aa:  ldloc.0
      IL_00ab:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       53 (0x35)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0025,
                            IL_0021,
                            IL_0027,
                            IL_0023)
      IL_001f:  br.s       IL_0027

      IL_0021:  br.s       IL_0029

      IL_0023:  br.s       IL_0029

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_0034

      .try
      {
        IL_0029:  leave.s    IL_0033

      }  // end .try
      finally
      {
        IL_002b:  ldarg.0
        IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>m__Finally46'()
        IL_0031:  nop
        IL_0032:  endfinally
      }  // end handler
      IL_0033:  nop
      IL_0034:  ret
    } // end of method '<YieldBreakInTryCatchInTryFinally>d__45'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::'<>1__state'
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>3__b'
      IL_003b:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::b
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       190 (0xbe)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1,
               bool V_2)
      .try
      {
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0027,
                              IL_0029,
                              IL_0023,
                              IL_0029,
                              IL_0025)
        IL_0021:  br.s       IL_0029

        IL_0023:  br.s       IL_0050

        IL_0025:  br.s       IL_009e

        IL_0027:  br.s       IL_002e

        IL_0029:  br         IL_00ae

        IL_002e:  ldarg.0
        IL_002f:  ldc.i4.m1
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_0035:  nop
        IL_0036:  nop
        IL_0037:  ldarg.0
        IL_0038:  ldc.i4.1
        IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_003e:  ldarg.0
        IL_003f:  ldc.i4.0
        IL_0040:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
        IL_0045:  ldarg.0
        IL_0046:  ldc.i4.2
        IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_004c:  ldc.i4.1
        IL_004d:  stloc.0
        IL_004e:  leave.s    IL_00bb

        IL_0050:  ldarg.0
        IL_0051:  ldc.i4.1
        IL_0052:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        .try
        {
          IL_0057:  nop
          IL_0058:  ldstr      "In Try"
          IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0062:  nop
          IL_0063:  ldarg.0
          IL_0064:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::b
          IL_0069:  ldc.i4.0
          IL_006a:  ceq
          IL_006c:  stloc.2
          IL_006d:  ldloc.2
          IL_006e:  brtrue.s   IL_007a

          IL_0070:  nop
          IL_0071:  ldarg.0
          IL_0072:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::System.IDisposable.Dispose()
          IL_0077:  nop
          IL_0078:  leave.s    IL_00ae

          IL_007a:  nop
          IL_007b:  leave.s    IL_008b

        }  // end .try
        finally
        {
          IL_007d:  nop
          IL_007e:  ldstr      "Inner Finally"
          IL_0083:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0088:  nop
          IL_0089:  nop
          IL_008a:  endfinally
        }  // end handler
        IL_008b:  nop
        IL_008c:  ldarg.0
        IL_008d:  ldc.i4.1
        IL_008e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
        IL_0093:  ldarg.0
        IL_0094:  ldc.i4.4
        IL_0095:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_009a:  ldc.i4.1
        IL_009b:  stloc.0
        IL_009c:  leave.s    IL_00bb

        IL_009e:  ldarg.0
        IL_009f:  ldc.i4.1
        IL_00a0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
        IL_00a5:  nop
        IL_00a6:  ldarg.0
        IL_00a7:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>m__Finally4a'()
        IL_00ac:  nop
        IL_00ad:  nop
        IL_00ae:  nop
        IL_00af:  ldc.i4.0
        IL_00b0:  stloc.0
        IL_00b1:  leave.s    IL_00bb

      }  // end .try
      fault
      {
        IL_00b3:  ldarg.0
        IL_00b4:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::System.IDisposable.Dispose()
        IL_00b9:  nop
        IL_00ba:  endfinally
      }  // end handler
      IL_00bb:  nop
      IL_00bc:  ldloc.0
      IL_00bd:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       53 (0x35)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0025,
                            IL_0021,
                            IL_0027,
                            IL_0023)
      IL_001f:  br.s       IL_0027

      IL_0021:  br.s       IL_0029

      IL_0023:  br.s       IL_0029

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_0034

      .try
      {
        IL_0029:  leave.s    IL_0033

      }  // end .try
      finally
      {
        IL_002b:  ldarg.0
        IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>m__Finally4a'()
        IL_0031:  nop
        IL_0032:  endfinally
      }  // end handler
      IL_0033:  nop
      IL_0034:  ret
    } // end of method '<YieldBreakInTryFinallyInTryFinally>d__49'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>1__state'
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<YieldBreakOnly>d__4d'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  ldc.i4.0
      IL_0009:  beq.s      IL_000d

      IL_000b:  br.s       IL_000f

      IL_000d:  br.s       IL_0011

      IL_000f:  br.s       IL_001b

      IL_0011:  ldarg.0
      IL_0012:  ldc.i4.m1
      IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>1__state'
      IL_0018:  nop
      IL_0019:  br.s       IL_001b

      IL_001b:  ldc.i4.0
      IL_001c:  stloc.0
      IL_001d:  br.s       IL_001f

      IL_001f:  ldloc.0
      IL_0020:  ret
    } // end of method '<YieldBreakOnly>d__4d'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<YieldBreakOnly>d__4d'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       95 (0x5f)
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
                              IL_001d,
                              IL_001f,
                              IL_001b)
        IL_0019:  br.s       IL_001f

        IL_001b:  br.s       IL_0043

        IL_001d:  br.s       IL_0021

        IL_001f:  br.s       IL_0050

        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.m1
        IL_0023:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0028:  nop
        IL_0029:  nop
        IL_002a:  ldarg.0
        IL_002b:  ldc.i4.1
        IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_0031:  ldarg.0
        IL_0032:  ldc.i4.0
        IL_0033:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>2__current'
        IL_0038:  ldarg.0
        IL_0039:  ldc.i4.2
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_003f:  ldc.i4.1
        IL_0040:  stloc.0
        IL_0041:  leave.s    IL_005c

        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.1
        IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
        IL_004a:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
        IL_004f:  throw

        IL_0050:  ldc.i4.0
        IL_0051:  stloc.0
        IL_0052:  leave.s    IL_005c

      }  // end .try
      fault
      {
        IL_0054:  ldarg.0
        IL_0055:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::System.IDisposable.Dispose()
        IL_005a:  nop
        IL_005b:  endfinally
      }  // end handler
      IL_005c:  nop
      IL_005d:  ldloc.0
      IL_005e:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001b,
                            IL_0019)
      IL_0017:  br.s       IL_001d

      IL_0019:  br.s       IL_001f

      IL_001b:  br.s       IL_001f

      IL_001d:  br.s       IL_002a

      .try
      {
        IL_001f:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0021:  ldarg.0
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>m__Finally51'()
        IL_0027:  nop
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  nop
      IL_002a:  ret
    } // end of method '<UnconditionalThrowInTryFinally>d__50'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::'<>1__state'
      IL_0008:  ldstr      "Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       104 (0x68)
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
        IL_0009:  beq.s      IL_0013

        IL_000b:  ldloc.1
        IL_000c:  ldc.i4.3
        IL_000d:  beq.s      IL_0011

        IL_000f:  br.s       IL_0015

        IL_0011:  br.s       IL_0041

        IL_0013:  br.s       IL_0017

        IL_0015:  br.s       IL_0059

        IL_0017:  ldarg.0
        IL_0018:  ldc.i4.m1
        IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_001e:  nop
        IL_001f:  nop
        IL_0020:  ldarg.0
        IL_0021:  ldc.i4.1
        IL_0022:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0027:  nop
        IL_0028:  ldarg.0
        IL_0029:  ldc.i4.2
        IL_002a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_002f:  ldarg.0
        IL_0030:  ldc.i4.0
        IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>2__current'
        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.3
        IL_0038:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_003d:  ldc.i4.1
        IL_003e:  stloc.0
        IL_003f:  leave.s    IL_0065

        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.2
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_0048:  nop
        IL_0049:  ldarg.0
        IL_004a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally56'()
        IL_004f:  nop
        IL_0050:  nop
        IL_0051:  ldarg.0
        IL_0052:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally55'()
        IL_0057:  nop
        IL_0058:  nop
        IL_0059:  ldc.i4.0
        IL_005a:  stloc.0
        IL_005b:  leave.s    IL_0065

      }  // end .try
      fault
      {
        IL_005d:  ldarg.0
        IL_005e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::System.IDisposable.Dispose()
        IL_0063:  nop
        IL_0064:  endfinally
      }  // end handler
      IL_0065:  nop
      IL_0066:  ldloc.0
      IL_0067:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       91 (0x5b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.1
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_0021,
                            IL_001f,
                            IL_001d)
      IL_001b:  br.s       IL_0023

      IL_001d:  br.s       IL_0025

      IL_001f:  br.s       IL_0025

      IL_0021:  br.s       IL_0025

      IL_0023:  br.s       IL_005a

      .try
      {
        IL_0025:  ldarg.0
        IL_0026:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
        IL_002b:  stloc.0
        IL_002c:  ldloc.0
        IL_002d:  ldc.i4.2
        IL_002e:  sub
        IL_002f:  switch     ( 
                              IL_0040,
                              IL_003e)
        IL_003c:  br.s       IL_0042

        IL_003e:  br.s       IL_0044

        IL_0040:  br.s       IL_0044

        IL_0042:  br.s       IL_004f

        .try
        {
          IL_0044:  leave.s    IL_004e

        }  // end .try
        finally
        {
          IL_0046:  ldarg.0
          IL_0047:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally56'()
          IL_004c:  nop
          IL_004d:  endfinally
        }  // end handler
        IL_004e:  nop
        IL_004f:  leave.s    IL_0059

      }  // end .try
      finally
      {
        IL_0051:  ldarg.0
        IL_0052:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally55'()
        IL_0057:  nop
        IL_0058:  endfinally
      }  // end handler
      IL_0059:  nop
      IL_005a:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0008:  ldstr      "Outer Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
    } // end of method '<NestedTryFinallyStartingOnSamePosition>d__54'::'<>m__Finally55'

    .method private hidebysig instance void 
            '<>m__Finally56'() cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::'<>1__state'
      IL_0008:  ldstr      "Inner Finally"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  ret
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
      // Code size       70 (0x46)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T> V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
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
      IL_0024:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>3__a'
      IL_003b:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::a
      IL_0040:  ldloc.0
      IL_0041:  stloc.1
      IL_0042:  br.s       IL_0044

      IL_0044:  ldloc.1
      IL_0045:  ret
    } // end of method '<LocalInFinally>d__59`1'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<LocalInFinally>d__59`1'::System.Collections.IEnumerable.GetEnumerator

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
        IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0006:  stloc.1
        IL_0007:  ldloc.1
        IL_0008:  switch     ( 
                              IL_0029,
                              IL_0023,
                              IL_002b,
                              IL_0025,
                              IL_0027)
        IL_0021:  br.s       IL_002b

        IL_0023:  br.s       IL_0047

        IL_0025:  br.s       IL_0068

        IL_0027:  br.s       IL_0089

        IL_0029:  br.s       IL_002d

        IL_002b:  br.s       IL_0091

        IL_002d:  ldarg.0
        IL_002e:  ldc.i4.m1
        IL_002f:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0034:  nop
        IL_0035:  ldarg.0
        IL_0036:  ldc.i4.1
        IL_0037:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
        IL_003c:  ldarg.0
        IL_003d:  ldc.i4.1
        IL_003e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0043:  ldc.i4.1
        IL_0044:  stloc.0
        IL_0045:  leave.s    IL_009d

        IL_0047:  ldarg.0
        IL_0048:  ldc.i4.m1
        IL_0049:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_004e:  nop
        IL_004f:  ldarg.0
        IL_0050:  ldc.i4.2
        IL_0051:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.2
        IL_0058:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.3
        IL_005f:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0064:  ldc.i4.1
        IL_0065:  stloc.0
        IL_0066:  leave.s    IL_009d

        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.2
        IL_006a:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_006f:  nop
        IL_0070:  ldarg.0
        IL_0071:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>m__Finally5a'()
        IL_0076:  nop
        IL_0077:  ldarg.0
        IL_0078:  ldc.i4.3
        IL_0079:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
        IL_007e:  ldarg.0
        IL_007f:  ldc.i4.4
        IL_0080:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0085:  ldc.i4.1
        IL_0086:  stloc.0
        IL_0087:  leave.s    IL_009d

        IL_0089:  ldarg.0
        IL_008a:  ldc.i4.m1
        IL_008b:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
        IL_0090:  nop
        IL_0091:  ldc.i4.0
        IL_0092:  stloc.0
        IL_0093:  leave.s    IL_009d

      }  // end .try
      fault
      {
        IL_0095:  ldarg.0
        IL_0096:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::System.IDisposable.Dispose()
        IL_009b:  nop
        IL_009c:  endfinally
      }  // end handler
      IL_009d:  nop
      IL_009e:  ldloc.0
      IL_009f:  ret
    } // end of method '<LocalInFinally>d__59`1'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       43 (0x2b)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  ldc.i4.2
      IL_0009:  sub
      IL_000a:  switch     ( 
                            IL_001b,
                            IL_0019)
      IL_0017:  br.s       IL_001d

      IL_0019:  br.s       IL_001f

      IL_001b:  br.s       IL_001f

      IL_001d:  br.s       IL_002a

      .try
      {
        IL_001f:  leave.s    IL_0029

      }  // end .try
      finally
      {
        IL_0021:  ldarg.0
        IL_0022:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>m__Finally5a'()
        IL_0027:  nop
        IL_0028:  endfinally
      }  // end handler
      IL_0029:  nop
      IL_002a:  ret
    } // end of method '<LocalInFinally>d__59`1'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
      // Code size       45 (0x2d)
      .maxstack  2
      .locals init (!T V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldc.i4.m1
      IL_0003:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::'<>1__state'
      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!T>::a
      IL_000e:  stloc.0
      IL_000f:  ldloca.s   V_0
      IL_0011:  constrained. !T
      IL_0017:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001c:  nop
      IL_001d:  ldloca.s   V_0
      IL_001f:  constrained. !T
      IL_0025:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002a:  nop
      IL_002b:  nop
      IL_002c:  ret
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
      // Code size       58 (0x3a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T> V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
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
      IL_0024:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0034

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  stloc.1
      IL_0036:  br.s       IL_0038

      IL_0038:  ldloc.1
      IL_0039:  ret
    } // end of method '<GenericYield>d__5d`1'::'System.Collections.Generic.IEnumerable<T>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'System.Collections.Generic.IEnumerable<T>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<GenericYield>d__5d`1'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       151 (0x97)
      .maxstack  3
      .locals init (bool V_0,
               int32 V_1,
               !T V_2,
               bool V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_0019,
                            IL_0017)
      IL_0015:  br.s       IL_001b

      IL_0017:  br.s       IL_006d

      IL_0019:  br.s       IL_001d

      IL_001b:  br.s       IL_0091

      IL_001d:  ldarg.0
      IL_001e:  ldc.i4.m1
      IL_001f:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldloca.s   V_2
      IL_0028:  initobj    !T
      IL_002e:  ldloc.2
      IL_002f:  box        !T
      IL_0034:  brfalse.s  IL_0041

      IL_0036:  ldloca.s   V_2
      IL_0038:  initobj    !T
      IL_003e:  ldloc.2
      IL_003f:  br.s       IL_0046

      IL_0041:  call       !!0 [mscorlib]System.Activator::CreateInstance<!T>()
      IL_0046:  nop
      IL_0047:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<val>5__5e'
      IL_004c:  ldarg.0
      IL_004d:  ldc.i4.0
      IL_004e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_0053:  br.s       IL_0083

      IL_0055:  nop
      IL_0056:  ldarg.0
      IL_0057:  ldarg.0
      IL_0058:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<val>5__5e'
      IL_005d:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>2__current'
      IL_0062:  ldarg.0
      IL_0063:  ldc.i4.1
      IL_0064:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0069:  ldc.i4.1
      IL_006a:  stloc.0
      IL_006b:  br.s       IL_0095

      IL_006d:  ldarg.0
      IL_006e:  ldc.i4.m1
      IL_006f:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>1__state'
      IL_0074:  nop
      IL_0075:  ldarg.0
      IL_0076:  dup
      IL_0077:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_007c:  ldc.i4.1
      IL_007d:  add
      IL_007e:  stfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_0083:  ldarg.0
      IL_0084:  ldfld      int32 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<i>5__5f'
      IL_0089:  ldc.i4.3
      IL_008a:  clt
      IL_008c:  stloc.3
      IL_008d:  ldloc.3
      IL_008e:  brtrue.s   IL_0055

      IL_0090:  nop
      IL_0091:  ldc.i4.0
      IL_0092:  stloc.0
      IL_0093:  br.s       IL_0095

      IL_0095:  ldloc.0
      IL_0096:  ret
    } // end of method '<GenericYield>d__5d`1'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance !T  'System.Collections.Generic.IEnumerator<T>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<!T>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (!T V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<GenericYield>d__5d`1'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!T>::'<>2__current'
      IL_0006:  box        !T
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<char> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<get_YieldChars>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
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
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturn>d__3'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::SimpleYieldReturn

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerator`1<string> 
          SimpleYieldReturnEnumerator() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<string> V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<SimpleYieldReturnEnumerator>d__6'::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.1
    IL_000c:  ret
  } // end of method YieldReturnPrettyTest::SimpleYieldReturnEnumerator

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnParameters(int32 p) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParameters>d__8' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
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
    IL_0017:  stloc.1
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.1
    IL_001b:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnParameters

  .method public hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
          YieldReturnParametersEnumerator(int32 p) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnParametersEnumerator>d__b' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1)
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
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnParametersEnumerator

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnInLoop() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnInLoop>d__d'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnInLoop

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldReturnWithTryFinally() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTryFinally>d__11'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          YieldReturnWithNestedTryFinally(bool breakInMiddle) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithNestedTryFinally>d__15'::'<>3__breakInMiddle'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithNestedTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<string> 
          YieldReturnWithTwoNonNestedFinallyBlocks(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<string> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithTwoNonNestedFinallyBlocks>d__1a'::'<>3__input'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithTwoNonNestedFinallyBlocks

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> 
          YieldReturnWithAnonymousMethods1(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods1>d__28'::'<>3__input'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithAnonymousMethods1

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> 
          YieldReturnWithAnonymousMethods2(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> input) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Func`1<string>> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldReturnWithAnonymousMethods2>d__30'::'<>3__input'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::YieldReturnWithAnonymousMethods2

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          GetEvenNumbers(int32 n) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GetEvenNumbers>d__36'::'<>3__n'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::GetEvenNumbers

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<char> 
          ExceptionHandling() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<char> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<ExceptionHandling>d__3a'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::ExceptionHandling

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInCatch() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatch>d__3e'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInCatch

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInCatchInTryFinally() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInCatchInTryFinally>d__41'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInCatchInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInTryCatchInTryFinally() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryCatchInTryFinally>d__45'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInTryCatchInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakInTryFinallyInTryFinally(bool b) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakInTryFinallyInTryFinally>d__49'::'<>3__b'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakInTryFinallyInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          YieldBreakOnly() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<YieldBreakOnly>d__4d'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::YieldBreakOnly

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          UnconditionalThrowInTryFinally() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<UnconditionalThrowInTryFinally>d__50'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::UnconditionalThrowInTryFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          NestedTryFinallyStartingOnSamePosition() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<NestedTryFinallyStartingOnSamePosition>d__54'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
  } // end of method YieldReturnPrettyTest::NestedTryFinallyStartingOnSamePosition

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          LocalInFinally<([mscorlib]System.IDisposable) T>(!!T a) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!!T> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!!T>::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<LocalInFinally>d__59`1'<!!T>::'<>3__a'
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method YieldReturnPrettyTest::LocalInFinally

  .method public hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T> 
          GenericYield<.ctor T>() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!!T> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<!!T> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.YieldReturnPrettyTest/'<GenericYield>d__5d`1'<!!T>::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.1
    IL_000d:  ret
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
      // Code size       82 (0x52)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               bool V_2)
      IL_0000:  call       int32 [mscorlib]System.Environment::get_CurrentManagedThreadId()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>l__initialThreadId'
      IL_000b:  bne.un.s   IL_001c

      IL_000d:  ldarg.0
      IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
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
      IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0029:  ldarg.0
      IL_002a:  stloc.0
      IL_002b:  br.s       IL_0040

      IL_002d:  ldc.i4.0
      IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::.ctor(int32)
      IL_0033:  stloc.0
      IL_0034:  ldloc.0
      IL_0035:  ldarg.0
      IL_0036:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_003b:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0040:  ldloc.0
      IL_0041:  ldarg.0
      IL_0042:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>3__<>4__this'
      IL_0047:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_004c:  ldloc.0
      IL_004d:  stloc.1
      IL_004e:  br.s       IL_0050

      IL_0050:  ldloc.1
      IL_0051:  ret
    } // end of method '<Count>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'

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
      IL_0001:  call       instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method '<Count>d__0'::System.Collections.IEnumerable.GetEnumerator

    .method private hidebysig newslot virtual final 
            instance bool  MoveNext() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerator::MoveNext
      // Code size       142 (0x8e)
      .maxstack  4
      .locals init (bool V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0006:  stloc.1
      IL_0007:  ldloc.1
      IL_0008:  switch     ( 
                            IL_001f,
                            IL_001b,
                            IL_001d)
      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_0052

      IL_001d:  br.s       IL_0080

      IL_001f:  br.s       IL_0023

      IL_0021:  br.s       IL_0088

      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.m1
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldarg.0
      IL_002d:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0032:  dup
      IL_0033:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0038:  dup
      IL_0039:  stloc.2
      IL_003a:  ldc.i4.1
      IL_003b:  add
      IL_003c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0041:  ldloc.2
      IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_0047:  ldarg.0
      IL_0048:  ldc.i4.1
      IL_0049:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_004e:  ldc.i4.1
      IL_004f:  stloc.0
      IL_0050:  br.s       IL_008c

      IL_0052:  ldarg.0
      IL_0053:  ldc.i4.m1
      IL_0054:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0059:  ldarg.0
      IL_005a:  ldarg.0
      IL_005b:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>4__this'
      IL_0060:  dup
      IL_0061:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_0066:  dup
      IL_0067:  stloc.2
      IL_0068:  ldc.i4.1
      IL_0069:  add
      IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn::val
      IL_006f:  ldloc.2
      IL_0070:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_0075:  ldarg.0
      IL_0076:  ldc.i4.2
      IL_0077:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_007c:  ldc.i4.1
      IL_007d:  stloc.0
      IL_007e:  br.s       IL_008c

      IL_0080:  ldarg.0
      IL_0081:  ldc.i4.m1
      IL_0082:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>1__state'
      IL_0087:  nop
      IL_0088:  ldc.i4.0
      IL_0089:  stloc.0
      IL_008a:  br.s       IL_008c

      IL_008c:  ldloc.0
      IL_008d:  ret
    } // end of method '<Count>d__0'::MoveNext

    .method private hidebysig newslot specialname virtual final 
            instance int32  'System.Collections.Generic.IEnumerator<System.Int32>.get_Current'() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override  method instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method '<Count>d__0'::System.IDisposable.Dispose

    .method private hidebysig newslot specialname virtual final 
            instance object  System.Collections.IEnumerator.get_Current() cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Collections.IEnumerator::get_Current
      // Code size       16 (0x10)
      .maxstack  1
      .locals init (object V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>2__current'
      IL_0006:  box        [mscorlib]System.Int32
      IL_000b:  stloc.0
      IL_000c:  br.s       IL_000e

      IL_000e:  ldloc.0
      IL_000f:  ret
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
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0' V_0,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_1)
    IL_0000:  ldc.i4.s   -2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::.ctor(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldarg.0
    IL_000a:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn
    IL_000f:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn/'<Count>d__0'::'<>3__<>4__this'
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.1
    IL_0019:  ret
  } // end of method StructWithYieldReturn::Count

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.StructWithYieldReturn


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
