
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
.assembly awc2kjfz
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
.module awc2kjfz.dll
// MVID: {49C8B10E-CA57-40C1-9484-BC8636C38817}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04D50000


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
        // Code size       20 (0x14)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::a
        IL_000d:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0012:  pop
        IL_0013:  ret
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
        // Code size       37 (0x25)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'CS$<>8__locals26'
        IL_0007:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::'<>4__this'
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::item
        IL_0012:  ldarg.0
        IL_0013:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'CS$<>8__locals26'
        IL_0018:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::a
        IL_001d:  add
        IL_001e:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0023:  pop
        IL_0024:  ret
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
        // Code size       49 (0x31)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2c'
        IL_0007:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::'<>4__this'
        IL_000c:  ldarg.0
        IL_000d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2e'
        IL_0012:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0017:  ldarg.0
        IL_0018:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2c'
        IL_001d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::a
        IL_0022:  add
        IL_0023:  ldarg.0
        IL_0024:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::copyOfItem
        IL_0029:  add
        IL_002a:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_002f:  pop
        IL_0030:  ret
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
        // Code size       51 (0x33)
        .maxstack  3
        .locals init (bool V_0)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
        IL_0007:  ldc.i4.0
        IL_0008:  clt
        IL_000a:  ldc.i4.0
        IL_000b:  ceq
        IL_000d:  stloc.0
        IL_000e:  ldloc.0
        IL_000f:  brtrue.s   IL_001a

        IL_0011:  nop
        IL_0012:  ldarg.0
        IL_0013:  ldc.i4.0
        IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
        IL_0019:  nop
        IL_001a:  ldarg.0
        IL_001b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<>4__this'
        IL_0020:  ldarg.0
        IL_0021:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<Bug951>b__37'()
        IL_0027:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0031:  nop
        IL_0032:  ret
      } // end of method '<>c__DisplayClass38'::'<Bug951>b__36'

      .method public hidebysig instance void 
              '<Bug951>b__37'() cil managed
      {
        // Code size       20 (0x14)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::amount
        IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0012:  nop
        IL_0013:  ret
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
        // Code size       51 (0x33)
        .maxstack  3
        .locals init (bool V_0)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
        IL_0007:  ldc.i4.0
        IL_0008:  clt
        IL_000a:  ldc.i4.0
        IL_000b:  ceq
        IL_000d:  stloc.0
        IL_000e:  ldloc.0
        IL_000f:  brtrue.s   IL_001a

        IL_0011:  nop
        IL_0012:  ldarg.0
        IL_0013:  ldc.i4.0
        IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
        IL_0019:  nop
        IL_001a:  ldarg.0
        IL_001b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<>4__this'
        IL_0020:  ldarg.0
        IL_0021:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<Bug951b>b__3b'()
        IL_0027:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_002c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0031:  nop
        IL_0032:  ret
      } // end of method '<>c__DisplayClass3c'::'<Bug951b>b__3a'

      .method public hidebysig instance void 
              '<Bug951b>b__3b'() cil managed
      {
        // Code size       20 (0x14)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
        IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0012:  nop
        IL_0013:  ret
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
        // Code size       26 (0x1a)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<Bug951c>b__3f'()
        IL_000e:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0018:  nop
        IL_0019:  ret
      } // end of method '<>c__DisplayClass40'::'<Bug951c>b__3e'

      .method public hidebysig instance void 
              '<Bug951c>b__3f'() cil managed
      {
        // Code size       25 (0x19)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::data
        IL_000d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData::Value
        IL_0012:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoSomething(string)
        IL_0017:  nop
        IL_0018:  ret
      } // end of method '<>c__DisplayClass40'::'<Bug951c>b__3f'

    } // end of class '<>c__DisplayClass40'

    .field private static class [mscorlib]System.Threading.ThreadStart 'CS$<>9__CachedAnonymousMethodDelegate35'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private static class [mscorlib]System.Action`1<object> 'CS$<>9__CachedAnonymousMethodDelegate43'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThis() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  2
      .locals init (class [mscorlib]System.Action V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<CaptureOfThis>b__20'()
      IL_0008:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_000d:  stloc.0
      IL_000e:  br.s       IL_0010

      IL_0010:  ldloc.0
      IL_0011:  ret
    } // end of method InstanceTests::CaptureOfThis

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameter(int32 a) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22' V_0,
               class [mscorlib]System.Action V_1)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::a
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::'<>4__this'
      IL_0014:  nop
      IL_0015:  ldloc.0
      IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass22'::'<CaptureOfThisAndParameter>b__21'()
      IL_001c:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0021:  stloc.1
      IL_0022:  br.s       IL_0024

      IL_0024:  ldloc.1
      IL_0025:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameter

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEach(int32 a) cil managed
    {
      // Code size       150 (0x96)
      .maxstack  2
      .locals init (class [mscorlib]System.Action V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28' V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' V_2,
               class [mscorlib]System.Action V_3,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_4,
               bool V_5)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::.ctor()
      IL_0005:  stloc.2
      IL_0006:  ldloc.2
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::a
      IL_000d:  ldloc.2
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25'::'<>4__this'
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_001b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0020:  stloc.s    V_4
      .try
      {
        IL_0022:  br.s       IL_006b

        IL_0024:  ldnull
        IL_0025:  stloc.0
        IL_0026:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::.ctor()
        IL_002b:  stloc.1
        IL_002c:  ldloc.1
        IL_002d:  ldloc.2
        IL_002e:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass25' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'CS$<>8__locals26'
        IL_0033:  ldloc.1
        IL_0034:  ldloc.s    V_4
        IL_0036:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::item
        IL_0040:  nop
        IL_0041:  ldloc.1
        IL_0042:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::item
        IL_0047:  ldc.i4.0
        IL_0048:  cgt
        IL_004a:  ldc.i4.0
        IL_004b:  ceq
        IL_004d:  stloc.s    V_5
        IL_004f:  ldloc.s    V_5
        IL_0051:  brtrue.s   IL_006a

        IL_0053:  nop
        IL_0054:  ldloc.0
        IL_0055:  brtrue.s   IL_0066

        IL_0057:  ldloc.1
        IL_0058:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass28'::'<CaptureOfThisAndParameterInForEach>b__24'()
        IL_005e:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0063:  stloc.0
        IL_0064:  br.s       IL_0066

        IL_0066:  ldloc.0
        IL_0067:  stloc.3
        IL_0068:  leave.s    IL_0093

        IL_006a:  nop
        IL_006b:  ldloc.s    V_4
        IL_006d:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0072:  stloc.s    V_5
        IL_0074:  ldloc.s    V_5
        IL_0076:  brtrue.s   IL_0024

        IL_0078:  leave.s    IL_008e

      }  // end .try
      finally
      {
        IL_007a:  ldloc.s    V_4
        IL_007c:  ldnull
        IL_007d:  ceq
        IL_007f:  stloc.s    V_5
        IL_0081:  ldloc.s    V_5
        IL_0083:  brtrue.s   IL_008d

        IL_0085:  ldloc.s    V_4
        IL_0087:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_008c:  nop
        IL_008d:  endfinally
      }  // end handler
      IL_008e:  nop
      IL_008f:  ldnull
      IL_0090:  stloc.3
      IL_0091:  br.s       IL_0093

      IL_0093:  nop
      IL_0094:  ldloc.3
      IL_0095:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEach

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEachWithItemCopy(int32 a) cil managed
    {
      // Code size       178 (0xb2)
      .maxstack  2
      .locals init (class [mscorlib]System.Action V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30' V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' V_3,
               class [mscorlib]System.Action V_4,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_5,
               bool V_6)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::.ctor()
      IL_0005:  stloc.3
      IL_0006:  ldloc.3
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::a
      IL_000d:  ldloc.3
      IL_000e:  ldarg.0
      IL_000f:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b'::'<>4__this'
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_001b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0020:  stloc.s    V_5
      .try
      {
        IL_0022:  br.s       IL_0085

        IL_0024:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::.ctor()
        IL_0029:  stloc.2
        IL_002a:  ldloc.2
        IL_002b:  ldloc.s    V_5
        IL_002d:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0037:  ldnull
        IL_0038:  stloc.0
        IL_0039:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::.ctor()
        IL_003e:  stloc.1
        IL_003f:  ldloc.1
        IL_0040:  ldloc.2
        IL_0041:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2e'
        IL_0046:  ldloc.1
        IL_0047:  ldloc.3
        IL_0048:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2b' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'CS$<>8__locals2c'
        IL_004d:  nop
        IL_004e:  ldloc.1
        IL_004f:  ldloc.2
        IL_0050:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0055:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::copyOfItem
        IL_005a:  ldloc.2
        IL_005b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2d'::item
        IL_0060:  ldc.i4.0
        IL_0061:  cgt
        IL_0063:  ldc.i4.0
        IL_0064:  ceq
        IL_0066:  stloc.s    V_6
        IL_0068:  ldloc.s    V_6
        IL_006a:  brtrue.s   IL_0084

        IL_006c:  nop
        IL_006d:  ldloc.0
        IL_006e:  brtrue.s   IL_007f

        IL_0070:  ldloc.1
        IL_0071:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass30'::'<CaptureOfThisAndParameterInForEachWithItemCopy>b__2a'()
        IL_0077:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_007c:  stloc.0
        IL_007d:  br.s       IL_007f

        IL_007f:  ldloc.0
        IL_0080:  stloc.s    V_4
        IL_0082:  leave.s    IL_00ae

        IL_0084:  nop
        IL_0085:  ldloc.s    V_5
        IL_0087:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_008c:  stloc.s    V_6
        IL_008e:  ldloc.s    V_6
        IL_0090:  brtrue.s   IL_0024

        IL_0092:  leave.s    IL_00a8

      }  // end .try
      finally
      {
        IL_0094:  ldloc.s    V_5
        IL_0096:  ldnull
        IL_0097:  ceq
        IL_0099:  stloc.s    V_6
        IL_009b:  ldloc.s    V_6
        IL_009d:  brtrue.s   IL_00a7

        IL_009f:  ldloc.s    V_5
        IL_00a1:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_00a6:  nop
        IL_00a7:  endfinally
      }  // end handler
      IL_00a8:  nop
      IL_00a9:  ldnull
      IL_00aa:  stloc.s    V_4
      IL_00ac:  br.s       IL_00ae

      IL_00ae:  nop
      IL_00af:  ldloc.s    V_4
      IL_00b1:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEachWithItemCopy

    .method public hidebysig instance void 
            LambdaInForLoop() cil managed
    {
      // Code size       53 (0x35)
      .maxstack  3
      .locals init (int32 V_0,
               class [mscorlib]System.Func`1<int32> V_1,
               bool V_2)
      IL_0000:  ldnull
      IL_0001:  stloc.1
      IL_0002:  nop
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  br.s       IL_0027

      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldloc.1
      IL_000a:  brtrue.s   IL_001b

      IL_000c:  ldarg.0
      IL_000d:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<LambdaInForLoop>b__32'()
      IL_0013:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                     native int)
      IL_0018:  stloc.1
      IL_0019:  br.s       IL_001b

      IL_001b:  ldloc.1
      IL_001c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Bar(class [mscorlib]System.Func`1<int32>)
      IL_0021:  nop
      IL_0022:  nop
      IL_0023:  ldloc.0
      IL_0024:  ldc.i4.1
      IL_0025:  add
      IL_0026:  stloc.0
      IL_0027:  ldloc.0
      IL_0028:  ldc.i4     0x186a0
      IL_002d:  clt
      IL_002f:  stloc.2
      IL_0030:  ldloc.2
      IL_0031:  brtrue.s   IL_0007

      IL_0033:  nop
      IL_0034:  ret
    } // end of method InstanceTests::LambdaInForLoop

    .method public hidebysig instance int32 
            Foo() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method InstanceTests::Foo

    .method public hidebysig instance void 
            Bar(class [mscorlib]System.Func`1<int32> f) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method InstanceTests::Bar

    .method private hidebysig instance void 
            Bug955() cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate35'
      IL_0006:  brtrue.s   IL_001b

      IL_0008:  ldnull
      IL_0009:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<Bug955>b__34'()
      IL_000f:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                       native int)
      IL_0014:  stsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate35'
      IL_0019:  br.s       IL_001b

      IL_001b:  ldsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate35'
      IL_0020:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
      IL_0025:  pop
      IL_0026:  ret
    } // end of method InstanceTests::Bug955

    .method public hidebysig instance void 
            Bug951(int32 amount) cil managed
    {
      // Code size       42 (0x2a)
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
      IL_0014:  nop
      IL_0015:  ldarg.0
      IL_0016:  ldloc.0
      IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass38'::'<Bug951>b__36'()
      IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0027:  nop
      IL_0028:  nop
      IL_0029:  ret
    } // end of method InstanceTests::Bug951

    .method public hidebysig instance void 
            Bug951b() cil managed
    {
      // Code size       47 (0x2f)
      .maxstack  3
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<>4__this'
      IL_000d:  nop
      IL_000e:  ldloc.0
      IL_000f:  ldarg.0
      IL_0010:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::amount
      IL_001a:  ldarg.0
      IL_001b:  ldloc.0
      IL_001c:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3c'::'<Bug951b>b__3a'()
      IL_0022:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0027:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_002c:  nop
      IL_002d:  nop
      IL_002e:  ret
    } // end of method InstanceTests::Bug951b

    .method public hidebysig instance void 
            Bug951c(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData data) cil managed
    {
      // Code size       42 (0x2a)
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
      IL_0014:  nop
      IL_0015:  ldarg.0
      IL_0016:  ldloc.0
      IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass40'::'<Bug951c>b__3e'()
      IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0027:  nop
      IL_0028:  nop
      IL_0029:  ret
    } // end of method InstanceTests::Bug951c

    .method public hidebysig instance class [mscorlib]System.Action`1<object> 
            Bug971_DelegateWithoutParameterList() cil managed
    {
      // Code size       37 (0x25)
      .maxstack  2
      .locals init (class [mscorlib]System.Action`1<object> V_0)
      IL_0000:  nop
      IL_0001:  ldsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate43'
      IL_0006:  brtrue.s   IL_001b

      IL_0008:  ldnull
      IL_0009:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<Bug971_DelegateWithoutParameterList>b__42'(object)
      IL_000f:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0014:  stsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate43'
      IL_0019:  br.s       IL_001b

      IL_001b:  ldsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'CS$<>9__CachedAnonymousMethodDelegate43'
      IL_0020:  stloc.0
      IL_0021:  br.s       IL_0023

      IL_0023:  ldloc.0
      IL_0024:  ret
    } // end of method InstanceTests::Bug971_DelegateWithoutParameterList

    .method private hidebysig instance void 
            DoAction(class [mscorlib]System.Action action) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method InstanceTests::DoAction

    .method private hidebysig instance void 
            NoOp(int32 a) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method InstanceTests::NoOp

    .method private hidebysig instance void 
            DoSomething(string text) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
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
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThis()
      IL_0007:  pop
      IL_0008:  ret
    } // end of method InstanceTests::'<CaptureOfThis>b__20'

    .method private hidebysig instance int32 
            '<LambdaInForLoop>b__32'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method InstanceTests::'<LambdaInForLoop>b__32'

    .method private hidebysig static void 
            '<Bug955>b__34'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method InstanceTests::'<Bug955>b__34'

    .method private hidebysig static void 
            '<Bug971_DelegateWithoutParameterList>b__42'(object param0) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
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
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1'::counter
      IL_0008:  ret
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
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5'::counter
      IL_0008:  ret
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
      // Code size       31 (0x1f)
      .maxstack  2
      .locals init (int32 V_0,
               bool V_1)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0011

      IL_0005:  nop
      IL_0006:  call       void [mscorlib]System.Console::WriteLine()
      IL_000b:  nop
      IL_000c:  nop
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  add
      IL_0010:  stloc.0
      IL_0011:  ldloc.0
      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
      IL_0018:  clt
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  brtrue.s   IL_0005

      IL_001e:  ret
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
        // Code size       25 (0x19)
        .maxstack  2
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::'CS$<>8__locals14'
        IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::a
        IL_000b:  ldarg.0
        IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15'::b
        IL_0011:  add
        IL_0012:  ldarg.1
        IL_0013:  add
        IL_0014:  stloc.0
        IL_0015:  br.s       IL_0017

        IL_0017:  ldloc.0
        IL_0018:  ret
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
      // Code size       37 (0x25)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'/'<>c__DisplayClass15' V_0,
               class [mscorlib]System.Func`2<int32,int32> V_1)
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
      IL_0020:  stloc.1
      IL_0021:  br.s       IL_0023

      IL_0023:  ldloc.1
      IL_0024:  ret
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
          // Code size       37 (0x25)
          .maxstack  2
          .locals init (int32 V_0)
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
          IL_0020:  stloc.0
          IL_0021:  br.s       IL_0023

          IL_0023:  ldloc.0
          IL_0024:  ret
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
        // Code size       49 (0x31)
        .maxstack  2
        .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c'/'<>c__DisplayClass1e' V_0,
                 class [mscorlib]System.Func`2<int32,int32> V_1)
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
        IL_002c:  stloc.1
        IL_002d:  br.s       IL_002f

        IL_002f:  ldloc.1
        IL_0030:  ret
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
      // Code size       37 (0x25)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'/'<>c__DisplayClass1c' V_0,
               class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> V_1)
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
      IL_0020:  stloc.1
      IL_0021:  br.s       IL_0023

      IL_0023:  ldloc.1
      IL_0024:  ret
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
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method DelegateConstruction::Test

  .method public hidebysig static class [mscorlib]System.Action`1<string> 
          ExtensionMethodUnbound() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (class [mscorlib]System.Action`1<string> V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::Test(string)
    IL_0008:  newobj     instance void class [mscorlib]System.Action`1<string>::.ctor(object,
                                                                                      native int)
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method DelegateConstruction::ExtensionMethodUnbound

  .method public hidebysig static class [mscorlib]System.Action 
          ExtensionMethodBound() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .locals init (class [mscorlib]System.Action V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "abc"
    IL_0006:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::Test(string)
    IL_000c:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method DelegateConstruction::ExtensionMethodBound

  .method public hidebysig static class [mscorlib]System.Action 
          ExtensionMethodBoundOnNull() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (class [mscorlib]System.Action V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::Test(string)
    IL_0008:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method DelegateConstruction::ExtensionMethodBoundOnNull

  .method public hidebysig static object 
          StaticMethod() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldftn      class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::ExtensionMethodBound()
    IL_0008:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Action>::.ctor(object,
                                                                                                           native int)
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method DelegateConstruction::StaticMethod

  .method public hidebysig static object 
          InstanceMethod() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "hello"
    IL_0006:  ldftn      instance string [mscorlib]System.String::ToUpper()
    IL_000c:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method DelegateConstruction::InstanceMethod

  .method public hidebysig static object 
          InstanceMethodOnNull() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldftn      instance string [mscorlib]System.String::ToUpper()
    IL_0008:  newobj     instance void class [mscorlib]System.Func`1<string>::.ctor(object,
                                                                                    native int)
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method DelegateConstruction::InstanceMethodOnNull

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> 
          AnonymousMethodStoreWithinLoop() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1' V_2,
             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_002a

    IL_000b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1'::.ctor()
    IL_0010:  stloc.2
    IL_0011:  nop
    IL_0012:  ldloc.0
    IL_0013:  ldloc.2
    IL_0014:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1'::'<AnonymousMethodStoreWithinLoop>b__0'(int32)
    IL_001a:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_001f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0024:  nop
    IL_0025:  nop
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.s   10
    IL_002d:  clt
    IL_002f:  stloc.s    V_4
    IL_0031:  ldloc.s    V_4
    IL_0033:  brtrue.s   IL_000b

    IL_0035:  ldloc.0
    IL_0036:  stloc.3
    IL_0037:  br.s       IL_0039

    IL_0039:  ldloc.3
    IL_003a:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreWithinLoop

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> 
          AnonymousMethodStoreOutsideLoop() cil managed
  {
    // Code size       70 (0x46)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class [mscorlib]System.Action`1<int32> V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5' V_3,
             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_4,
             bool V_5)
    IL_0000:  ldnull
    IL_0001:  stloc.2
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5'::.ctor()
    IL_0007:  stloc.3
    IL_0008:  nop
    IL_0009:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_000e:  stloc.0
    IL_000f:  ldc.i4.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0033

    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  ldloc.2
    IL_0016:  brtrue.s   IL_0027

    IL_0018:  ldloc.3
    IL_0019:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass5'::'<AnonymousMethodStoreOutsideLoop>b__3'(int32)
    IL_001f:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0024:  stloc.2
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.2
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_002d:  nop
    IL_002e:  nop
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  stloc.1
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.s   10
    IL_0036:  clt
    IL_0038:  stloc.s    V_5
    IL_003a:  ldloc.s    V_5
    IL_003c:  brtrue.s   IL_0013

    IL_003e:  ldloc.0
    IL_003f:  stloc.s    V_4
    IL_0041:  br.s       IL_0043

    IL_0043:  ldloc.s    V_4
    IL_0045:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreOutsideLoop

  .method public hidebysig static class [mscorlib]System.Action 
          StaticAnonymousMethodNoClosure() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (class [mscorlib]System.Action V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'<StaticAnonymousMethodNoClosure>b__7'()
    IL_000f:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0014:  stsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.0
    IL_0024:  ret
  } // end of method DelegateConstruction::StaticAnonymousMethodNoClosure

  .method public hidebysig static void  NameConflict() cil managed
  {
    // Code size       104 (0x68)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class [mscorlib]System.Action`1<int32> V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb' V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_005c

    IL_000b:  ldnull
    IL_000c:  stloc.2
    IL_000d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::.ctor()
    IL_0012:  stloc.3
    IL_0013:  nop
    IL_0014:  ldloc.3
    IL_0015:  ldc.i4.0
    IL_0016:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_001b:  br.s       IL_0047

    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  ldloc.2
    IL_0020:  brtrue.s   IL_0031

    IL_0022:  ldloc.3
    IL_0023:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::'<NameConflict>b__9'(int32)
    IL_0029:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_002e:  stloc.2
    IL_002f:  br.s       IL_0031

    IL_0031:  ldloc.2
    IL_0032:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldloc.3
    IL_003a:  dup
    IL_003b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_0047:  ldloc.3
    IL_0048:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClassb'::i
    IL_004d:  ldc.i4.s   10
    IL_004f:  clt
    IL_0051:  stloc.s    V_4
    IL_0053:  ldloc.s    V_4
    IL_0055:  brtrue.s   IL_001d

    IL_0057:  nop
    IL_0058:  ldloc.1
    IL_0059:  ldc.i4.1
    IL_005a:  add
    IL_005b:  stloc.1
    IL_005c:  ldloc.1
    IL_005d:  ldc.i4.s   10
    IL_005f:  clt
    IL_0061:  stloc.s    V_4
    IL_0063:  ldloc.s    V_4
    IL_0065:  brtrue.s   IL_000b

    IL_0067:  ret
  } // end of method DelegateConstruction::NameConflict

  .method public hidebysig static void  NameConflict2(int32 j) cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0037

    IL_000b:  nop
    IL_000c:  ldloc.0
    IL_000d:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_0012:  brtrue.s   IL_0027

    IL_0014:  ldnull
    IL_0015:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'<NameConflict2>b__d'(int32)
    IL_001b:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0020:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_0025:  br.s       IL_0027

    IL_0027:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_002c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.1
    IL_0035:  add
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  clt
    IL_003c:  stloc.2
    IL_003d:  ldloc.2
    IL_003e:  brtrue.s   IL_000b

    IL_0040:  ret
  } // end of method DelegateConstruction::NameConflict2

  .method public hidebysig static class [mscorlib]System.Action`1<int32> 
          NameConflict3(int32 i) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (class [mscorlib]System.Action`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_0006:  brtrue.s   IL_001b

    IL_0008:  ldnull
    IL_0009:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'<NameConflict3>b__f'(int32)
    IL_000f:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0014:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_0019:  br.s       IL_001b

    IL_001b:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.0
    IL_0024:  ret
  } // end of method DelegateConstruction::NameConflict3

  .method public hidebysig static class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> 
          CurriedAddition(int32 a) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13' V_0,
             class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::a
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass13'::'<CurriedAddition>b__11'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>::.ctor(object,
                                                                                                                              native int)
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.1
    IL_001e:  ret
  } // end of method DelegateConstruction::CurriedAddition

  .method public hidebysig static class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>> 
          CurriedAddition2(int32 a) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a' V_0,
             class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::a
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass1a'::'<CurriedAddition2>b__17'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.1
    IL_001e:  ret
  } // end of method DelegateConstruction::CurriedAddition2

  .method private hidebysig static void  '<StaticAnonymousMethodNoClosure>b__7'() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       void [mscorlib]System.Console::WriteLine()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method DelegateConstruction::'<StaticAnonymousMethodNoClosure>b__7'

  .method private hidebysig static void  '<NameConflict2>b__d'(int32 i) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0007:  nop
    IL_0008:  ret
  } // end of method DelegateConstruction::'<NameConflict2>b__d'

  .method private hidebysig static void  '<NameConflict3>b__f'(int32 j) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0012

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  clt
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  brtrue.s   IL_0005

    IL_001a:  ret
  } // end of method DelegateConstruction::'<NameConflict3>b__f'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\DelegateConstruction.res
