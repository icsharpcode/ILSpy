
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
.assembly DelegateConstruction
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
.module DelegateConstruction.dll
// MVID: {BCE55681-8792-49AA-A3A8-0599BF49400D}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04FD0000


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

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2_0'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 a
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass2_0'::.ctor

      .method assembly hidebysig instance void 
              '<CaptureOfThisAndParameter>b__0'() cil managed
      {
        // Code size       19 (0x13)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::a
        IL_000c:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0011:  pop
        IL_0012:  ret
      } // end of method '<>c__DisplayClass2_0'::'<CaptureOfThisAndParameter>b__0'

    } // end of class '<>c__DisplayClass2_0'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass3_0'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 item
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' 'CS$<>8__locals1'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass3_0'::.ctor

      .method assembly hidebysig instance void 
              '<CaptureOfThisAndParameterInForEach>b__0'() cil managed
      {
        // Code size       36 (0x24)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'CS$<>8__locals1'
        IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::'<>4__this'
        IL_000b:  ldarg.0
        IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::item
        IL_0011:  ldarg.0
        IL_0012:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'CS$<>8__locals1'
        IL_0017:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::a
        IL_001c:  add
        IL_001d:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0022:  pop
        IL_0023:  ret
      } // end of method '<>c__DisplayClass3_0'::'<CaptureOfThisAndParameterInForEach>b__0'

    } // end of class '<>c__DisplayClass3_0'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass3_1'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 a
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass3_1'::.ctor

    } // end of class '<>c__DisplayClass3_1'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4_0'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 item
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' 'CS$<>8__locals1'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass4_0'::.ctor

    } // end of class '<>c__DisplayClass4_0'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4_1'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 a
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass4_1'::.ctor

    } // end of class '<>c__DisplayClass4_1'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4_2'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 copyOfItem
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' 'CS$<>8__locals2'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass4_2'::.ctor

      .method assembly hidebysig instance void 
              '<CaptureOfThisAndParameterInForEachWithItemCopy>b__0'() cil managed
      {
        // Code size       58 (0x3a)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::'CS$<>8__locals1'
        IL_000b:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::'<>4__this'
        IL_0010:  ldarg.0
        IL_0011:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0016:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_001b:  ldarg.0
        IL_001c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0021:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::'CS$<>8__locals1'
        IL_0026:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::a
        IL_002b:  add
        IL_002c:  ldarg.0
        IL_002d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::copyOfItem
        IL_0032:  add
        IL_0033:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0038:  pop
        IL_0039:  ret
      } // end of method '<>c__DisplayClass4_2'::'<CaptureOfThisAndParameterInForEachWithItemCopy>b__0'

    } // end of class '<>c__DisplayClass4_2'

    .class auto ansi serializable sealed nested private beforefieldinit '<>c'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c' '<>9'
      .field public static class [mscorlib]System.Threading.ThreadStart '<>9__8_0'
      .field public static class [mscorlib]System.Action`1<object> '<>9__12_0'
      .method private hidebysig specialname rtspecialname static 
              void  .cctor() cil managed
      {
        // Code size       11 (0xb)
        .maxstack  8
        IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::.ctor()
        IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9'
        IL_000a:  ret
      } // end of method '<>c'::.cctor

      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c'::.ctor

      .method assembly hidebysig instance void 
              '<Bug955>b__8_0'() cil managed
      {
        // Code size       1 (0x1)
        .maxstack  8
        IL_0000:  ret
      } // end of method '<>c'::'<Bug955>b__8_0'

      .method assembly hidebysig instance void 
              '<Bug971_DelegateWithoutParameterList>b__12_0'(object '<p0>') cil managed
      {
        // Code size       1 (0x1)
        .maxstack  8
        IL_0000:  ret
      } // end of method '<>c'::'<Bug971_DelegateWithoutParameterList>b__12_0'

    } // end of class '<>c'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass9_0'
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
      } // end of method '<>c__DisplayClass9_0'::.ctor

      .method assembly hidebysig instance void 
              '<Bug951>b__0'() cil managed
      {
        // Code size       40 (0x28)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
        IL_0006:  ldc.i4.0
        IL_0007:  bge.s      IL_0010

        IL_0009:  ldarg.0
        IL_000a:  ldc.i4.0
        IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
        IL_0010:  ldarg.0
        IL_0011:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<>4__this'
        IL_0016:  ldarg.0
        IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<Bug951>b__1'()
        IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0027:  ret
      } // end of method '<>c__DisplayClass9_0'::'<Bug951>b__0'

      .method assembly hidebysig instance void 
              '<Bug951>b__1'() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
        IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0011:  ret
      } // end of method '<>c__DisplayClass9_0'::'<Bug951>b__1'

    } // end of class '<>c__DisplayClass9_0'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass10_0'
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
      } // end of method '<>c__DisplayClass10_0'::.ctor

      .method assembly hidebysig instance void 
              '<Bug951b>b__0'() cil managed
      {
        // Code size       40 (0x28)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
        IL_0006:  ldc.i4.0
        IL_0007:  bge.s      IL_0010

        IL_0009:  ldarg.0
        IL_000a:  ldc.i4.0
        IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
        IL_0010:  ldarg.0
        IL_0011:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<>4__this'
        IL_0016:  ldarg.0
        IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<Bug951b>b__1'()
        IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0027:  ret
      } // end of method '<>c__DisplayClass10_0'::'<Bug951b>b__0'

      .method assembly hidebysig instance void 
              '<Bug951b>b__1'() cil managed
      {
        // Code size       18 (0x12)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
        IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0011:  ret
      } // end of method '<>c__DisplayClass10_0'::'<Bug951b>b__1'

    } // end of class '<>c__DisplayClass10_0'

    .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass11_0'
           extends [mscorlib]System.Object
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData data
      .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests '<>4__this'
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method '<>c__DisplayClass11_0'::.ctor

      .method assembly hidebysig instance void 
              '<Bug951c>b__0'() cil managed
      {
        // Code size       24 (0x18)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<Bug951c>b__1'()
        IL_000d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0012:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0017:  ret
      } // end of method '<>c__DisplayClass11_0'::'<Bug951c>b__0'

      .method assembly hidebysig instance void 
              '<Bug951c>b__1'() cil managed
      {
        // Code size       23 (0x17)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<>4__this'
        IL_0006:  ldarg.0
        IL_0007:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::data
        IL_000c:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData::Value
        IL_0011:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoSomething(string)
        IL_0016:  ret
      } // end of method '<>c__DisplayClass11_0'::'<Bug951c>b__1'

    } // end of class '<>c__DisplayClass11_0'

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThis() cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<CaptureOfThis>b__1_0'()
      IL_0007:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_000c:  ret
    } // end of method InstanceTests::CaptureOfThis

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameter(int32 a) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::.ctor()
      IL_0005:  dup
      IL_0006:  ldarg.0
      IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::'<>4__this'
      IL_000c:  dup
      IL_000d:  ldarg.1
      IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::a
      IL_0013:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::'<CaptureOfThisAndParameter>b__0'()
      IL_0019:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_001e:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameter

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEach(int32 a) cil managed
    {
      // Code size       106 (0x6a)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0' V_2,
               class [mscorlib]System.Action V_3)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::a
      IL_0014:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_0019:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_001e:  stloc.1
      .try
      {
        IL_001f:  br.s       IL_0052

        IL_0021:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::.ctor()
        IL_0026:  stloc.2
        IL_0027:  ldloc.2
        IL_0028:  ldloc.0
        IL_0029:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'CS$<>8__locals1'
        IL_002e:  ldloc.2
        IL_002f:  ldloc.1
        IL_0030:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::item
        IL_003a:  ldloc.2
        IL_003b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::item
        IL_0040:  ldc.i4.0
        IL_0041:  ble.s      IL_0052

        IL_0043:  ldloc.2
        IL_0044:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'<CaptureOfThisAndParameterInForEach>b__0'()
        IL_004a:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_004f:  stloc.3
        IL_0050:  leave.s    IL_0068

        IL_0052:  ldloc.1
        IL_0053:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0058:  brtrue.s   IL_0021

        IL_005a:  leave.s    IL_0066

      }  // end .try
      finally
      {
        IL_005c:  ldloc.1
        IL_005d:  brfalse.s  IL_0065

        IL_005f:  ldloc.1
        IL_0060:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0065:  endfinally
      }  // end handler
      IL_0066:  ldnull
      IL_0067:  ret

      IL_0068:  ldloc.3
      IL_0069:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEach

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEachWithItemCopy(int32 a) cil managed
    {
      // Code size       143 (0x8f)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2' V_3,
               class [mscorlib]System.Action V_4)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::a
      IL_0014:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_0019:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_001e:  stloc.1
      .try
      {
        IL_001f:  br.s       IL_0076

        IL_0021:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::.ctor()
        IL_0026:  stloc.2
        IL_0027:  ldloc.2
        IL_0028:  ldloc.0
        IL_0029:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::'CS$<>8__locals1'
        IL_002e:  ldloc.2
        IL_002f:  ldloc.1
        IL_0030:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0035:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_003a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::.ctor()
        IL_003f:  stloc.3
        IL_0040:  ldloc.3
        IL_0041:  ldloc.2
        IL_0042:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0047:  ldloc.3
        IL_0048:  ldloc.3
        IL_0049:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_004e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_0053:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::copyOfItem
        IL_0058:  ldloc.3
        IL_0059:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_005e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_0063:  ldc.i4.0
        IL_0064:  ble.s      IL_0076

        IL_0066:  ldloc.3
        IL_0067:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'<CaptureOfThisAndParameterInForEachWithItemCopy>b__0'()
        IL_006d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0072:  stloc.s    V_4
        IL_0074:  leave.s    IL_008c

        IL_0076:  ldloc.1
        IL_0077:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_007c:  brtrue.s   IL_0021

        IL_007e:  leave.s    IL_008a

      }  // end .try
      finally
      {
        IL_0080:  ldloc.1
        IL_0081:  brfalse.s  IL_0089

        IL_0083:  ldloc.1
        IL_0084:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0089:  endfinally
      }  // end handler
      IL_008a:  ldnull
      IL_008b:  ret

      IL_008c:  ldloc.s    V_4
      IL_008e:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEachWithItemCopy

    .method public hidebysig instance void 
            LambdaInForLoop() cil managed
    {
      // Code size       35 (0x23)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  br.s       IL_001a

      IL_0004:  ldarg.0
      IL_0005:  ldarg.0
      IL_0006:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<LambdaInForLoop>b__5_0'()
      IL_000c:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                     native int)
      IL_0011:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Bar(class [mscorlib]System.Func`1<int32>)
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.1
      IL_0018:  add
      IL_0019:  stloc.0
      IL_001a:  ldloc.0
      IL_001b:  ldc.i4     0x186a0
      IL_0020:  blt.s      IL_0004

      IL_0022:  ret
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
      // Code size       38 (0x26)
      .maxstack  8
      IL_0000:  ldsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__8_0'
      IL_0005:  dup
      IL_0006:  brtrue.s   IL_001f

      IL_0008:  pop
      IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9'
      IL_000e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<Bug955>b__8_0'()
      IL_0014:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                       native int)
      IL_0019:  dup
      IL_001a:  stsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__8_0'
      IL_001f:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
      IL_0024:  pop
      IL_0025:  ret
    } // end of method InstanceTests::Bug955

    .method public hidebysig instance void 
            Bug951(int32 amount) cil managed
    {
      // Code size       39 (0x27)
      .maxstack  3
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
      IL_0014:  ldarg.0
      IL_0015:  ldloc.0
      IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<Bug951>b__0'()
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
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.0
      IL_000f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
      IL_0019:  ldarg.0
      IL_001a:  ldloc.0
      IL_001b:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<Bug951b>b__0'()
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
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::data
      IL_0014:  ldarg.0
      IL_0015:  ldloc.0
      IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<Bug951c>b__0'()
      IL_001c:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0026:  ret
    } // end of method InstanceTests::Bug951c

    .method public hidebysig instance class [mscorlib]System.Action`1<object> 
            Bug971_DelegateWithoutParameterList() cil managed
    {
      // Code size       32 (0x20)
      .maxstack  8
      IL_0000:  ldsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__12_0'
      IL_0005:  dup
      IL_0006:  brtrue.s   IL_001f

      IL_0008:  pop
      IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9'
      IL_000e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<Bug971_DelegateWithoutParameterList>b__12_0'(object)
      IL_0014:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_0019:  dup
      IL_001a:  stsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__12_0'
      IL_001f:  ret
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
            '<CaptureOfThis>b__1_0'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThis()
      IL_0006:  pop
      IL_0007:  ret
    } // end of method InstanceTests::'<CaptureOfThis>b__1_0'

    .method private hidebysig instance int32 
            '<LambdaInForLoop>b__5_0'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0006:  ret
    } // end of method InstanceTests::'<LambdaInForLoop>b__5_0'

  } // end of class InstanceTests

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass8_0'
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
    } // end of method '<>c__DisplayClass8_0'::.ctor

    .method assembly hidebysig instance void 
            '<AnonymousMethodStoreWithinLoop>b__0'(int32 x) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0'::counter
      IL_0007:  ret
    } // end of method '<>c__DisplayClass8_0'::'<AnonymousMethodStoreWithinLoop>b__0'

  } // end of class '<>c__DisplayClass8_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass9_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 counter
    .field public class [mscorlib]System.Action`1<int32> '<>9__0'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass9_0'::.ctor

    .method assembly hidebysig instance void 
            '<AnonymousMethodStoreOutsideLoop>b__0'(int32 x) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::counter
      IL_0007:  ret
    } // end of method '<>c__DisplayClass9_0'::'<AnonymousMethodStoreOutsideLoop>b__0'

  } // end of class '<>c__DisplayClass9_0'

  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' '<>9'
    .field public static class [mscorlib]System.Action '<>9__10_0'
    .field public static class [mscorlib]System.Action`1<int32> '<>9__12_0'
    .field public static class [mscorlib]System.Action`1<int32> '<>9__13_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
      IL_000a:  ret
    } // end of method '<>c'::.cctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c'::.ctor

    .method assembly hidebysig instance void 
            '<StaticAnonymousMethodNoClosure>b__10_0'() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  call       void [mscorlib]System.Console::WriteLine()
      IL_0005:  ret
    } // end of method '<>c'::'<StaticAnonymousMethodNoClosure>b__10_0'

    .method assembly hidebysig instance void 
            '<NameConflict2>b__12_0'(int32 i) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0006:  ret
    } // end of method '<>c'::'<NameConflict2>b__12_0'

    .method assembly hidebysig instance void 
            '<NameConflict3>b__13_0'(int32 j) cil managed
    {
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
      IL_000f:  ldarg.1
      IL_0010:  blt.s      IL_0004

      IL_0012:  ret
    } // end of method '<>c'::'<NameConflict3>b__13_0'

  } // end of class '<>c'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass11_0'
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
    } // end of method '<>c__DisplayClass11_0'::.ctor

    .method assembly hidebysig instance void 
            '<NameConflict>b__0'(int32 j) cil managed
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
      IL_000f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
      IL_0014:  blt.s      IL_0004

      IL_0016:  ret
    } // end of method '<>c__DisplayClass11_0'::'<NameConflict>b__0'

  } // end of class '<>c__DisplayClass11_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass14_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass14_0'::.ctor

    .method assembly hidebysig instance class [mscorlib]System.Func`2<int32,int32> 
            '<CurriedAddition>b__0'(int32 b) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::.ctor()
      IL_0005:  dup
      IL_0006:  ldarg.0
      IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::'CS$<>8__locals1'
      IL_000c:  dup
      IL_000d:  ldarg.1
      IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::b
      IL_0013:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::'<CurriedAddition>b__1'(int32)
      IL_0019:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_001e:  ret
    } // end of method '<>c__DisplayClass14_0'::'<CurriedAddition>b__0'

  } // end of class '<>c__DisplayClass14_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass14_1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 b
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0' 'CS$<>8__locals1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass14_1'::.ctor

    .method assembly hidebysig instance int32 
            '<CurriedAddition>b__1'(int32 c) cil managed
    {
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::'CS$<>8__locals1'
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::a
      IL_000b:  ldarg.0
      IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::b
      IL_0011:  add
      IL_0012:  ldarg.1
      IL_0013:  add
      IL_0014:  ret
    } // end of method '<>c__DisplayClass14_1'::'<CurriedAddition>b__1'

  } // end of class '<>c__DisplayClass14_1'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass15_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass15_0'::.ctor

    .method assembly hidebysig instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> 
            '<CurriedAddition2>b__0'(int32 b) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::.ctor()
      IL_0005:  dup
      IL_0006:  ldarg.0
      IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_000c:  dup
      IL_000d:  ldarg.1
      IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::b
      IL_0013:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::'<CurriedAddition2>b__1'(int32)
      IL_0019:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>::.ctor(object,
                                                                                                                                native int)
      IL_001e:  ret
    } // end of method '<>c__DisplayClass15_0'::'<CurriedAddition2>b__0'

  } // end of class '<>c__DisplayClass15_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass15_1'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 b
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0' 'CS$<>8__locals1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass15_1'::.ctor

    .method assembly hidebysig instance class [mscorlib]System.Func`2<int32,int32> 
            '<CurriedAddition2>b__1'(int32 c) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::.ctor()
      IL_0005:  dup
      IL_0006:  ldarg.0
      IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::'CS$<>8__locals2'
      IL_000c:  dup
      IL_000d:  ldarg.1
      IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::c
      IL_0013:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::'<CurriedAddition2>b__2'(int32)
      IL_0019:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_001e:  ret
    } // end of method '<>c__DisplayClass15_1'::'<CurriedAddition2>b__1'

  } // end of class '<>c__DisplayClass15_1'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass15_2'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 c
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1' 'CS$<>8__locals2'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass15_2'::.ctor

    .method assembly hidebysig instance int32 
            '<CurriedAddition2>b__2'(int32 d) cil managed
    {
      // Code size       38 (0x26)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::'CS$<>8__locals2'
      IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::a
      IL_0010:  ldarg.0
      IL_0011:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::'CS$<>8__locals2'
      IL_0016:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::b
      IL_001b:  add
      IL_001c:  ldarg.0
      IL_001d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::c
      IL_0022:  add
      IL_0023:  ldarg.1
      IL_0024:  add
      IL_0025:  ret
    } // end of method '<>c__DisplayClass15_2'::'<CurriedAddition2>b__2'

  } // end of class '<>c__DisplayClass15_2'

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
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0' V_2)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0026

    IL_000a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0'::.ctor()
    IL_000f:  stloc.2
    IL_0010:  ldloc.0
    IL_0011:  ldloc.2
    IL_0012:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0'::'<AnonymousMethodStoreWithinLoop>b__0'(int32)
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
    // Code size       64 (0x40)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0' V_0,
             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_1,
             int32 V_2,
             class [mscorlib]System.Action`1<int32> V_3)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.2
    IL_000e:  br.s       IL_0039

    IL_0010:  ldloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldfld      class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::'<>9__0'
    IL_0017:  dup
    IL_0018:  brtrue.s   IL_0030

    IL_001a:  pop
    IL_001b:  ldloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::'<AnonymousMethodStoreOutsideLoop>b__0'(int32)
    IL_0023:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0028:  dup
    IL_0029:  stloc.3
    IL_002a:  stfld      class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::'<>9__0'
    IL_002f:  ldloc.3
    IL_0030:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0035:  ldloc.2
    IL_0036:  ldc.i4.1
    IL_0037:  add
    IL_0038:  stloc.2
    IL_0039:  ldloc.2
    IL_003a:  ldc.i4.s   10
    IL_003c:  blt.s      IL_0010

    IL_003e:  ldloc.1
    IL_003f:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreOutsideLoop

  .method public hidebysig static class [mscorlib]System.Action 
          StaticAnonymousMethodNoClosure() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__10_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
    IL_000e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<StaticAnonymousMethodNoClosure>b__10_0'()
    IL_0014:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__10_0'
    IL_001f:  ret
  } // end of method DelegateConstruction::StaticAnonymousMethodNoClosure

  .method public hidebysig static void  NameConflict() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0' V_2,
             int32 V_3)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0049

    IL_000a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::.ctor()
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.0
    IL_0012:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0017:  br.s       IL_003b

    IL_0019:  ldloc.0
    IL_001a:  ldloc.2
    IL_001b:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::'<NameConflict>b__0'(int32)
    IL_0021:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0026:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_002b:  ldloc.2
    IL_002c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0031:  stloc.3
    IL_0032:  ldloc.2
    IL_0033:  ldloc.3
    IL_0034:  ldc.i4.1
    IL_0035:  add
    IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_003b:  ldloc.2
    IL_003c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0041:  ldc.i4.s   10
    IL_0043:  blt.s      IL_0019

    IL_0045:  ldloc.1
    IL_0046:  ldc.i4.1
    IL_0047:  add
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.s   10
    IL_004c:  blt.s      IL_000a

    IL_004e:  ret
  } // end of method DelegateConstruction::NameConflict

  .method public hidebysig static void  NameConflict2(int32 j) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0033

    IL_000a:  ldloc.0
    IL_000b:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__12_0'
    IL_0010:  dup
    IL_0011:  brtrue.s   IL_002a

    IL_0013:  pop
    IL_0014:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
    IL_0019:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<NameConflict2>b__12_0'(int32)
    IL_001f:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0024:  dup
    IL_0025:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__12_0'
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  stloc.1
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.s   10
    IL_0036:  blt.s      IL_000a

    IL_0038:  ret
  } // end of method DelegateConstruction::NameConflict2

  .method public hidebysig static class [mscorlib]System.Action`1<int32> 
          NameConflict3(int32 i) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__13_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
    IL_000e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<NameConflict3>b__13_0'(int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__13_0'
    IL_001f:  ret
  } // end of method DelegateConstruction::NameConflict3

  .method public hidebysig static class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> 
          CurriedAddition(int32 a) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::a
    IL_000c:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::'<CurriedAddition>b__0'(int32)
    IL_0012:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>::.ctor(object,
                                                                                                                              native int)
    IL_0017:  ret
  } // end of method DelegateConstruction::CurriedAddition

  .method public hidebysig static class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>> 
          CurriedAddition2(int32 a) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::.ctor()
    IL_0005:  dup
    IL_0006:  ldarg.0
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::a
    IL_000c:  ldftn      instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::'<CurriedAddition2>b__0'(int32)
    IL_0012:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_0017:  ret
  } // end of method DelegateConstruction::CurriedAddition2

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
