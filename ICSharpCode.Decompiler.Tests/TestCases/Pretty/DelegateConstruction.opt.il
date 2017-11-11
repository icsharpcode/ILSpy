
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ci3jaj4f
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ci3jaj4f.dll
// MVID: {F860E23B-9B90-453A-A3A5-49C4D5AA91F8}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05340000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .class auto ansi nested private beforefieldinit InstanceTests
         extends [mscorlib]System.Object
  {
    .class sequential ansi sealed nested public beforefieldinit SomeData
           extends [mscorlib]System.ValueType
    {
      .field public string Value
    } // end of class SomeData

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass22'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .field public int32 a
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass22'::.ctor

      .method public hidebysig instance void 
              '<CaptureOfThisAndParameter>b__21'() cil managed
      {
        // Code size       19 (0x13)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::a
        IL_000c:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0011:  pop
        IL_0012:  ret
      } // end of method '<>c__DisplayClass22'::'<CaptureOfThisAndParameter>b__21'

    } // end of class '<>c__DisplayClass22'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass25'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .field public int32 a
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass25'::.ctor

    } // end of class '<>c__DisplayClass25'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass28'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' 'CS$<>8__locals26'
      .field public int32 item
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass28'::.ctor

      .method public hidebysig instance void 
              '<CaptureOfThisAndParameterInForEach>b__24'() cil managed
      {
        // Code size       36 (0x24)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'CS$<>8__locals26'
        IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::'<>4__this'
        IL_000b:  ldarg.0
        IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::item
        IL_0011:  ldarg.0
        IL_0012:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'CS$<>8__locals26'
        IL_0017:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::a
        IL_001c:  add
        IL_001d:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0022:  pop
        IL_0023:  ret
      } // end of method '<>c__DisplayClass28'::'<CaptureOfThisAndParameterInForEach>b__24'

    } // end of class '<>c__DisplayClass28'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2b'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .field public int32 a
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass2b'::.ctor

    } // end of class '<>c__DisplayClass2b'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2d'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 item
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass2d'::.ctor

    } // end of class '<>c__DisplayClass2d'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass30'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' 'CS$<>8__locals2e'
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' 'CS$<>8__locals2c'
      .field public int32 copyOfItem
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass30'::.ctor

      .method public hidebysig instance void 
              '<CaptureOfThisAndParameterInForEachWithItemCopy>b__2a'() cil managed
      {
        // Code size       48 (0x30)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2c'
        IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::'<>4__this'
        IL_000b:  ldarg.0
        IL_000c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2e'
        IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0016:  ldarg.0
        IL_0017:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2c'
        IL_001c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::a
        IL_0021:  add
        IL_0022:  ldarg.0
        IL_0023:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::copyOfItem
        IL_0028:  add
        IL_0029:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_002e:  pop
        IL_002f:  ret
      } // end of method '<>c__DisplayClass30'::'<CaptureOfThisAndParameterInForEachWithItemCopy>b__2a'

    } // end of class '<>c__DisplayClass30'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass38'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .field public int32 amount
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass38'::.ctor

      .method public hidebysig instance void 
              '<Bug951>b__36'() cil managed
      {
        // Code size       40 (0x28)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
        IL_0006:  ldc.i4.0
        IL_0007:  bge.s      IL_0010

        IL_0009:  ldarg.0
        IL_000a:  ldc.i4.0
        IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
        IL_0010:  ldarg.0
        IL_0011:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<>4__this'
        IL_0016:  ldarg.0
        IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<Bug951>b__37'()
        IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0027:  ret
      } // end of method '<>c__DisplayClass38'::'<Bug951>b__36'

      .method public hidebysig instance void 
              '<Bug951>b__37'() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
        IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0011:  ret
      } // end of method '<>c__DisplayClass38'::'<Bug951>b__37'

    } // end of class '<>c__DisplayClass38'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass3c'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 amount
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass3c'::.ctor

      .method public hidebysig instance void 
              '<Bug951b>b__3a'() cil managed
      {
        // Code size       40 (0x28)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
        IL_0006:  ldc.i4.0
        IL_0007:  bge.s      IL_0010

        IL_0009:  ldarg.0
        IL_000a:  ldc.i4.0
        IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
        IL_0010:  ldarg.0
        IL_0011:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<>4__this'
        IL_0016:  ldarg.0
        IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<Bug951b>b__3b'()
        IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0027:  ret
      } // end of method '<>c__DisplayClass3c'::'<Bug951b>b__3a'

      .method public hidebysig instance void 
              '<Bug951b>b__3b'() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
        IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0011:  ret
      } // end of method '<>c__DisplayClass3c'::'<Bug951b>b__3b'

    } // end of class '<>c__DisplayClass3c'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass40'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData data
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass40'::.ctor

      .method public hidebysig instance void 
              '<Bug951c>b__3e'() cil managed
      {
        // Code size       24 (0x18)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<Bug951c>b__3f'()
        IL_000d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0012:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0017:  ret
      } // end of method '<>c__DisplayClass40'::'<Bug951c>b__3e'

      .method public hidebysig instance void 
              '<Bug951c>b__3f'() cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::data
        IL_000c:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData::Value
        IL_0011:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoSomething(string)
        IL_0016:  ret
      } // end of method '<>c__DisplayClass40'::'<Bug951c>b__3f'

    } // end of class '<>c__DisplayClass40'

    .field private static class [mscorlib]System.Threading.ThreadStart 'CS$<>9__CachedAnonymousMethodDelegate35'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private static class [mscorlib]System.Action`1<object> 'CS$<>9__CachedAnonymousMethodDelegate43'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThis() cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<CaptureOfThis>b__20'()
      IL_0007:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_000c:  ret
    } // end of method InstanceTests::CaptureOfThis

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameter(int32 a) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::a
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::'<>4__this'
      IL_0014:  ldloc.0
      IL_0015:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::'<CaptureOfThisAndParameter>b__21'()
      IL_001b:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0020:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameter

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEach(int32 a) cil managed
    {
      // Code size       118 (0x76)
      .maxstack  2
      .locals init (class [mscorlib]System.Action V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28' V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' V_2,
               class [mscorlib]System.Action V_3,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_4)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::.ctor()
      IL_0005:  stloc.2
      IL_0006:  ldloc.2
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::a
      IL_000d:  ldloc.2
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::'<>4__this'
      IL_0014:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_0019:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_001e:  stloc.s    V_4
      .try
      {
        IL_0020:  br.s       IL_005b

        IL_0022:  ldnull
        IL_0023:  stloc.0
        IL_0024:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::.ctor()
        IL_0029:  stloc.1
        IL_002a:  ldloc.1
        IL_002b:  ldloc.2
        IL_002c:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'CS$<>8__locals26'
        IL_0031:  ldloc.1
        IL_0032:  ldloc.s    V_4
        IL_0034:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0039:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::item
        IL_003e:  ldloc.1
        IL_003f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::item
        IL_0044:  ldc.i4.0
        IL_0045:  ble.s      IL_005b

        IL_0047:  ldloc.0
        IL_0048:  brtrue.s   IL_0057

        IL_004a:  ldloc.1
        IL_004b:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'<CaptureOfThisAndParameterInForEach>b__24'()
        IL_0051:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0056:  stloc.0
        IL_0057:  ldloc.0
        IL_0058:  stloc.3
        IL_0059:  leave.s    IL_0074

        IL_005b:  ldloc.s    V_4
        IL_005d:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0062:  brtrue.s   IL_0022

        IL_0064:  leave.s    IL_0072

      }  // end .try
      finally
      {
        IL_0066:  ldloc.s    V_4
        IL_0068:  brfalse.s  IL_0071

        IL_006a:  ldloc.s    V_4
        IL_006c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0071:  endfinally
      }  // end handler
      IL_0072:  ldnull
      IL_0073:  ret

      IL_0074:  ldloc.3
      IL_0075:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEach

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEachWithItemCopy(int32 a) cil managed
    {
      // Code size       145 (0x91)
      .maxstack  2
      .locals init (class [mscorlib]System.Action V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30' V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' V_3,
               class [mscorlib]System.Action V_4,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_5)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::.ctor()
      IL_0005:  stloc.3
      IL_0006:  ldloc.3
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::a
      IL_000d:  ldloc.3
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::'<>4__this'
      IL_0014:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_0019:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_001e:  stloc.s    V_5
      .try
      {
        IL_0020:  br.s       IL_0075

        IL_0022:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::.ctor()
        IL_0027:  stloc.2
        IL_0028:  ldloc.2
        IL_0029:  ldloc.s    V_5
        IL_002b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0035:  ldnull
        IL_0036:  stloc.0
        IL_0037:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::.ctor()
        IL_003c:  stloc.1
        IL_003d:  ldloc.1
        IL_003e:  ldloc.2
        IL_003f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2e'
        IL_0044:  ldloc.1
        IL_0045:  ldloc.3
        IL_0046:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2c'
        IL_004b:  ldloc.1
        IL_004c:  ldloc.2
        IL_004d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0052:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::copyOfItem
        IL_0057:  ldloc.2
        IL_0058:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_005d:  ldc.i4.0
        IL_005e:  ble.s      IL_0075

        IL_0060:  ldloc.0
        IL_0061:  brtrue.s   IL_0070

        IL_0063:  ldloc.1
        IL_0064:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'<CaptureOfThisAndParameterInForEachWithItemCopy>b__2a'()
        IL_006a:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_006f:  stloc.0
        IL_0070:  ldloc.0
        IL_0071:  stloc.s    V_4
        IL_0073:  leave.s    IL_008e

        IL_0075:  ldloc.s    V_5
        IL_0077:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_007c:  brtrue.s   IL_0022

        IL_007e:  leave.s    IL_008c

      }  // end .try
      finally
      {
        IL_0080:  ldloc.s    V_5
        IL_0082:  brfalse.s  IL_008b

        IL_0084:  ldloc.s    V_5
        IL_0086:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_008b:  endfinally
      }  // end handler
      IL_008c:  ldnull
      IL_008d:  ret

      IL_008e:  ldloc.s    V_4
      IL_0090:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEachWithItemCopy

    .method public hidebysig instance void 
            LambdaInForLoop() cil managed
    {
      // Code size       42 (0x2a)
      .maxstack  3
      .locals init (int32 V_0,
               class [mscorlib]System.Func`1<int32> V_1)
      IL_0000:  ldnull
      IL_0001:  stloc.1
      IL_0002:  ldc.i4.0
      IL_0003:  stloc.0
      IL_0004:  br.s       IL_0021

      IL_0006:  ldarg.0
      IL_0007:  ldloc.1
      IL_0008:  brtrue.s   IL_0017

      IL_000a:  ldarg.0
      IL_000b:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<LambdaInForLoop>b__32'()
      IL_0011:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                     native int)
      IL_0016:  stloc.1
      IL_0017:  ldloc.1
      IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Bar(class [mscorlib]System.Func`1<int32>)
      IL_001d:  ldloc.0
      IL_001e:  ldc.i4.1
      IL_001f:  add
      IL_0020:  stloc.0
      IL_0021:  ldloc.0
      IL_0022:  ldc.i4     0x186a0
      IL_0027:  blt.s      IL_0006

      IL_0029:  ret
    } // end of method InstanceTests::LambdaInForLoop

    .method public hidebysig instance int32 
            Foo() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method InstanceTests::Foo

    .method public hidebysig instance void 
            Bar(class [mscorlib]System.Func`1<int32> f) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method InstanceTests::Bar

    .method private hidebysig instance void 
            Bug955() cil managed
    {
      // Code size       36 (0x24)
      .maxstack  8
      IL_0000:  ldsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate35'
      IL_0005:  brtrue.s   IL_0018

      IL_0007:  ldnull
      IL_0008:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<Bug955>b__34'()
      IL_000e:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                       native int)
      IL_0013:  stsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate35'
      IL_0018:  ldsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate35'
      IL_001d:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
      IL_0022:  pop
      IL_0023:  ret
    } // end of method InstanceTests::Bug955

    .method public hidebysig instance void 
            Bug951(int32 amount) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  3
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<>4__this'
      IL_0014:  ldarg.0
      IL_0015:  ldloc.0
      IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<Bug951>b__36'()
      IL_001c:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0026:  ret
    } // end of method InstanceTests::Bug951

    .method public hidebysig instance void 
            Bug951b() cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  3
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
      IL_0019:  ldarg.0
      IL_001a:  ldloc.0
      IL_001b:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<Bug951b>b__3a'()
      IL_0021:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0026:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_002b:  ret
    } // end of method InstanceTests::Bug951b

    .method public hidebysig instance void 
            Bug951c(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData data) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  3
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::data
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<>4__this'
      IL_0014:  ldarg.0
      IL_0015:  ldloc.0
      IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<Bug951c>b__3e'()
      IL_001c:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0026:  ret
    } // end of method InstanceTests::Bug951c

    .method public hidebysig instance class [mscorlib]System.Action`1<object> 
            Bug971_DelegateWithoutParameterList() cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  8
      IL_0000:  ldsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate43'
      IL_0005:  brtrue.s   IL_0018

      IL_0007:  ldnull
      IL_0008:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<Bug971_DelegateWithoutParameterList>b__42'(object)
      IL_000e:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0013:  stsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate43'
      IL_0018:  ldsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate43'
      IL_001d:  ret
    } // end of method InstanceTests::Bug971_DelegateWithoutParameterList

    .method private hidebysig instance void 
            DoAction(class [mscorlib]System.Action action) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method InstanceTests::DoAction

    .method private hidebysig instance void 
            NoOp(int32 a) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method InstanceTests::NoOp

    .method private hidebysig instance void 
            DoSomething(string text) cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method InstanceTests::DoSomething

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method InstanceTests::.ctor

    .method private hidebysig instance void 
            '<CaptureOfThis>b__20'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThis()
      IL_0006:  pop
      IL_0007:  ret
    } // end of method InstanceTests::'<CaptureOfThis>b__20'

    .method private hidebysig instance int32 
            '<LambdaInForLoop>b__32'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0006:  ret
    } // end of method InstanceTests::'<LambdaInForLoop>b__32'

    .method private hidebysig static void 
            '<Bug955>b__34'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method InstanceTests::'<Bug955>b__34'

    .method private hidebysig static void 
            '<Bug971_DelegateWithoutParameterList>b__42'(object param0) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method InstanceTests::'<Bug971_DelegateWithoutParameterList>b__42'

  } // end of class InstanceTests

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 counter
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
            '<AnonymousMethodStoreWithinLoop>b__0'(int32 x) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1'::counter
      IL_0007:  ret
    } // end of method '<>c__DisplayClass1'::'<AnonymousMethodStoreWithinLoop>b__0'

  } // end of class '<>c__DisplayClass1'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass5'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 counter
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass5'::.ctor

    .method public hidebysig instance void 
            '<AnonymousMethodStoreOutsideLoop>b__3'(int32 x) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5'::counter
      IL_0007:  ret
    } // end of method '<>c__DisplayClass5'::'<AnonymousMethodStoreOutsideLoop>b__3'

  } // end of class '<>c__DisplayClass5'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClassb'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 i
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClassb'::.ctor

    .method public hidebysig instance void 
            '<NameConflict>b__9'(int32 j) cil managed
    {
      // Code size       23 (0x17)
      .maxstack  2
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_000d

      IL_0004:  call       void [mscorlib]System.Console::WriteLine()
      IL_0009:  ldloc.0
      IL_000a:  ldarg.1
      IL_000b:  add
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
      IL_0014:  blt.s      IL_0004

      IL_0016:  ret
    } // end of method '<>c__DisplayClassb'::'<NameConflict>b__9'

  } // end of class '<>c__DisplayClassb'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass13'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass15'
           extends [mscorlib]System.Object
    {
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13' 'CS$<>8__locals14'
      .field public int32 b
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass15'::.ctor

      .method public hidebysig instance int32 
              '<CurriedAddition>b__12'(int32 c) cil managed
      {
        // Code size       21 (0x15)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::'CS$<>8__locals14'
        IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::a
        IL_000b:  ldarg.0
        IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::b
        IL_0011:  add
        IL_0012:  ldarg.1
        IL_0013:  add
        IL_0014:  ret
      } // end of method '<>c__DisplayClass15'::'<CurriedAddition>b__12'

    } // end of class '<>c__DisplayClass15'

    .field public int32 a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass13'::.ctor

    .method public hidebysig instance class [mscorlib]System.Func`2<int32,int32> 
            '<CurriedAddition>b__11'(int32 b) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::'CS$<>8__locals14'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::b
      IL_0014:  ldloc.0
      IL_0015:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::'<CurriedAddition>b__12'(int32)
      IL_001b:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_0020:  ret
    } // end of method '<>c__DisplayClass13'::'<CurriedAddition>b__11'

  } // end of class '<>c__DisplayClass13'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1a'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1c'
           extends [mscorlib]System.Object
    {
      .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1e'
             extends [mscorlib]System.Object
      {
        .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c' 'CS$<>8__locals1d'
        .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' 'CS$<>8__locals1b'
        .field public int32 c
        .method public hidebysig specialname rtspecialname 
                instance void  .ctor() cil managed
        {
          // Code size       7 (0x7)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
          IL_0006:  ret
        } // end of method '<>c__DisplayClass1e'::.ctor

        .method public hidebysig instance int32 
                '<CurriedAddition2>b__19'(int32 d) cil managed
        {
          // Code size       33 (0x21)
          .maxstack  8
          IL_0000:  ldarg.0
          IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::'CS$<>8__locals1b'
          IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::a
          IL_000b:  ldarg.0
          IL_000c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::'CS$<>8__locals1d'
          IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'::b
          IL_0016:  add
          IL_0017:  ldarg.0
          IL_0018:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::c
          IL_001d:  add
          IL_001e:  ldarg.1
          IL_001f:  add
          IL_0020:  ret
        } // end of method '<>c__DisplayClass1e'::'<CurriedAddition2>b__19'

      } // end of class '<>c__DisplayClass1e'

      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' 'CS$<>8__locals1b'
      .field public int32 b
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass1c'::.ctor

      .method public hidebysig instance class [mscorlib]System.Func`2<int32,int32> 
              '<CurriedAddition2>b__18'(int32 c) cil managed
      {
        // Code size       45 (0x2d)
        .maxstack  2
        .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e' V_0)
        IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::.ctor()
        IL_0005:  stloc.0
        IL_0006:  ldloc.0
        IL_0007:  ldarg.0
        IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::'CS$<>8__locals1d'
        IL_000d:  ldloc.0
        IL_000e:  ldarg.0
        IL_000f:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'::'CS$<>8__locals1b'
        IL_0014:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::'CS$<>8__locals1b'
        IL_0019:  ldloc.0
        IL_001a:  ldarg.1
        IL_001b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::c
        IL_0020:  ldloc.0
        IL_0021:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e'::'<CurriedAddition2>b__19'(int32)
        IL_0027:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                             native int)
        IL_002c:  ret
      } // end of method '<>c__DisplayClass1c'::'<CurriedAddition2>b__18'

    } // end of class '<>c__DisplayClass1c'

    .field public int32 a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1a'::.ctor

    .method public hidebysig instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> 
            '<CurriedAddition2>b__17'(int32 b) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'::'CS$<>8__locals1b'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'::b
      IL_0014:  ldloc.0
      IL_0015:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'::'<CurriedAddition2>b__18'(int32)
      IL_001b:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>::.ctor(object,
                                                                                                                                native int)
      IL_0020:  ret
    } // end of method '<>c__DisplayClass1a'::'<CurriedAddition2>b__17'

  } // end of class '<>c__DisplayClass1a'

  .field private static class [mscorlib]System.Action 'CS$<>9__CachedAnonymousMethodDelegate8'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Action`1<int32> 'CS$<>9__CachedAnonymousMethodDelegatee'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Action`1<int32> 'CS$<>9__CachedAnonymousMethodDelegate10'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig static void  Test(string a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method DelegateConstruction::Test

  .method public hidebysig static class [mscorlib]System.Action`1<string> 
          ExtensionMethodUnbound() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::Test(string)
    IL_0007:  newobj     instance void class [mscorlib]System.Action`1<string>::.ctor(object,
                                                                                      native int)
    IL_000c:  ret
  } // end of method DelegateConstruction::ExtensionMethodUnbound

  .method public hidebysig static class [mscorlib]System.Action 
          ExtensionMethodBound() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldstr      "abc"
    IL_0005:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::Test(string)
    IL_000b:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0010:  ret
  } // end of method DelegateConstruction::ExtensionMethodBound

  .method public hidebysig static class [mscorlib]System.Action 
          ExtensionMethodBoundOnNull() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::Test(string)
    IL_0007:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_000c:  ret
  } // end of method DelegateConstruction::ExtensionMethodBoundOnNull

  .method public hidebysig static object 
          StaticMethod() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::ExtensionMethodBound()
    IL_0007:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Action>::.ctor(object,
                                                                                                           native int)
    IL_000c:  ret
  } // end of method DelegateConstruction::StaticMethod

  .method public hidebysig static object 
          InstanceMethod() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldstr      "hello"
    IL_0005:  ldftn      instance string [mscorlib]System.String::ToUpper()
    IL_000b:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_0010:  ret
  } // end of method DelegateConstruction::InstanceMethod

  .method public hidebysig static object 
          InstanceMethodOnNull() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      instance string [mscorlib]System.String::ToUpper()
    IL_0007:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_000c:  ret
  } // end of method DelegateConstruction::InstanceMethodOnNull

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> 
          AnonymousMethodStoreWithinLoop() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1' V_2)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0026

    IL_000a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1'::.ctor()
    IL_000f:  stloc.2
    IL_0010:  ldloc.0
    IL_0011:  ldloc.2
    IL_0012:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1'::'<AnonymousMethodStoreWithinLoop>b__0'(int32)
    IL_0018:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.s   10
    IL_0029:  blt.s      IL_000a

    IL_002b:  ldloc.0
    IL_002c:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreWithinLoop

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> 
          AnonymousMethodStoreOutsideLoop() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class [mscorlib]System.Action`1<int32> V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5' V_3)
    IL_0000:  ldnull
    IL_0001:  stloc.2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5'::.ctor()
    IL_0007:  stloc.3
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  stloc.1
    IL_0010:  br.s       IL_002d

    IL_0012:  ldloc.0
    IL_0013:  ldloc.2
    IL_0014:  brtrue.s   IL_0023

    IL_0016:  ldloc.3
    IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5'::'<AnonymousMethodStoreOutsideLoop>b__3'(int32)
    IL_001d:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0022:  stloc.2
    IL_0023:  ldloc.2
    IL_0024:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  add
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.s   10
    IL_0030:  blt.s      IL_0012

    IL_0032:  ldloc.0
    IL_0033:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreOutsideLoop

  .method public hidebysig static class [mscorlib]System.Action 
          StaticAnonymousMethodNoClosure() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'<StaticAnonymousMethodNoClosure>b__7'()
    IL_000e:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0013:  stsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0018:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_001d:  ret
  } // end of method DelegateConstruction::StaticAnonymousMethodNoClosure

  .method public hidebysig static void  NameConflict() cil managed
  {
    // Code size       84 (0x54)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class [mscorlib]System.Action`1<int32> V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb' V_3)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_004e

    IL_000a:  ldnull
    IL_000b:  stloc.2
    IL_000c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::.ctor()
    IL_0011:  stloc.3
    IL_0012:  ldloc.3
    IL_0013:  ldc.i4.0
    IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_0019:  br.s       IL_0040

    IL_001b:  ldloc.0
    IL_001c:  ldloc.2
    IL_001d:  brtrue.s   IL_002c

    IL_001f:  ldloc.3
    IL_0020:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::'<NameConflict>b__9'(int32)
    IL_0026:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0032:  ldloc.3
    IL_0033:  dup
    IL_0034:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_0039:  ldc.i4.1
    IL_003a:  add
    IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_0040:  ldloc.3
    IL_0041:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_0046:  ldc.i4.s   10
    IL_0048:  blt.s      IL_001b

    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.1
    IL_004c:  add
    IL_004d:  stloc.1
    IL_004e:  ldloc.1
    IL_004f:  ldc.i4.s   10
    IL_0051:  blt.s      IL_000a

    IL_0053:  ret
  } // end of method DelegateConstruction::NameConflict

  .method public hidebysig static void  NameConflict2(int32 j) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0031

    IL_000a:  ldloc.0
    IL_000b:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_0010:  brtrue.s   IL_0023

    IL_0012:  ldnull
    IL_0013:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'<NameConflict2>b__d'(int32)
    IL_0019:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_001e:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_0023:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.s   10
    IL_0034:  blt.s      IL_000a

    IL_0036:  ret
  } // end of method DelegateConstruction::NameConflict2

  .method public hidebysig static class [mscorlib]System.Action`1<int32> 
          NameConflict3(int32 i) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_0005:  brtrue.s   IL_0018

    IL_0007:  ldnull
    IL_0008:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'<NameConflict3>b__f'(int32)
    IL_000e:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0013:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_0018:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_001d:  ret
  } // end of method DelegateConstruction::NameConflict3

  .method public hidebysig static class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> 
          CurriedAddition(int32 a) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::a
    IL_000d:  ldloc.0
    IL_000e:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::'<CurriedAddition>b__11'(int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>::.ctor(object,
                                                                                                                              native int)
    IL_0019:  ret
  } // end of method DelegateConstruction::CurriedAddition

  .method public hidebysig static class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>> 
          CurriedAddition2(int32 a) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::a
    IL_000d:  ldloc.0
    IL_000e:  ldftn      instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::'<CurriedAddition2>b__17'(int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_0019:  ret
  } // end of method DelegateConstruction::CurriedAddition2

  .method private hidebysig static void  '<StaticAnonymousMethodNoClosure>b__7'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       void [mscorlib]System.Console::WriteLine()
    IL_0005:  ret
  } // end of method DelegateConstruction::'<StaticAnonymousMethodNoClosure>b__7'

  .method private hidebysig static void  '<NameConflict2>b__d'(int32 i) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0006:  ret
  } // end of method DelegateConstruction::'<NameConflict2>b__d'

  .method private hidebysig static void  '<NameConflict3>b__f'(int32 j) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       19 (0x13)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_000e

    IL_0004:  ldloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldarg.0
    IL_0010:  blt.s      IL_0004

    IL_0012:  ret
  } // end of method DelegateConstruction::'<NameConflict3>b__f'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\DelegateConstruction.opt.res
