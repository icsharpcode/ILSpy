
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module DelegateConstruction.dll
// MVID: {06FD5D29-DFCF-456A-A796-15359E05F003}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x011B0000


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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c__DisplayClass2_0'::.ctor

      .method assembly hidebysig instance void 
              '<CaptureOfThisAndParameter>b__0'() cil managed
      {
        // Code size       20 (0x14)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::a
        IL_000d:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0012:  pop
        IL_0013:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c__DisplayClass3_0'::.ctor

      .method assembly hidebysig instance void 
              '<CaptureOfThisAndParameterInForEach>b__0'() cil managed
      {
        // Code size       37 (0x25)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'CS$<>8__locals1'
        IL_0007:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::'<>4__this'
        IL_000c:  ldarg.0
        IL_000d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::item
        IL_0012:  ldarg.0
        IL_0013:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'CS$<>8__locals1'
        IL_0018:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::a
        IL_001d:  add
        IL_001e:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0023:  pop
        IL_0024:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c__DisplayClass4_2'::.ctor

      .method assembly hidebysig instance void 
              '<CaptureOfThisAndParameterInForEachWithItemCopy>b__0'() cil managed
      {
        // Code size       59 (0x3b)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0007:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::'CS$<>8__locals1'
        IL_000c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::'<>4__this'
        IL_0011:  ldarg.0
        IL_0012:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0017:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_001c:  ldarg.0
        IL_001d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0022:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::'CS$<>8__locals1'
        IL_0027:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::a
        IL_002c:  add
        IL_002d:  ldarg.0
        IL_002e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::copyOfItem
        IL_0033:  add
        IL_0034:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThisAndParameter(int32)
        IL_0039:  pop
        IL_003a:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c'::.ctor

      .method assembly hidebysig instance void 
              '<Bug955>b__8_0'() cil managed
      {
        // Code size       2 (0x2)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ret
      } // end of method '<>c'::'<Bug955>b__8_0'

      .method assembly hidebysig instance void 
              '<Bug971_DelegateWithoutParameterList>b__12_0'(object '<p0>') cil managed
      {
        // Code size       2 (0x2)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c__DisplayClass9_0'::.ctor

      .method assembly hidebysig instance void 
              '<Bug951>b__0'() cil managed
      {
        // Code size       48 (0x30)
        .maxstack  3
        .locals init (bool V_0)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
        IL_0007:  ldc.i4.0
        IL_0008:  clt
        IL_000a:  stloc.0
        IL_000b:  ldloc.0
        IL_000c:  brfalse.s  IL_0017

        IL_000e:  nop
        IL_000f:  ldarg.0
        IL_0010:  ldc.i4.0
        IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
        IL_0016:  nop
        IL_0017:  ldarg.0
        IL_0018:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<>4__this'
        IL_001d:  ldarg.0
        IL_001e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<Bug951>b__1'()
        IL_0024:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0029:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_002e:  nop
        IL_002f:  ret
      } // end of method '<>c__DisplayClass9_0'::'<Bug951>b__0'

      .method assembly hidebysig instance void 
              '<Bug951>b__1'() cil managed
      {
        // Code size       20 (0x14)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::amount
        IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0012:  nop
        IL_0013:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c__DisplayClass10_0'::.ctor

      .method assembly hidebysig instance void 
              '<Bug951b>b__0'() cil managed
      {
        // Code size       48 (0x30)
        .maxstack  3
        .locals init (bool V_0)
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
        IL_0007:  ldc.i4.0
        IL_0008:  clt
        IL_000a:  stloc.0
        IL_000b:  ldloc.0
        IL_000c:  brfalse.s  IL_0017

        IL_000e:  nop
        IL_000f:  ldarg.0
        IL_0010:  ldc.i4.0
        IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
        IL_0016:  nop
        IL_0017:  ldarg.0
        IL_0018:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<>4__this'
        IL_001d:  ldarg.0
        IL_001e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<Bug951b>b__1'()
        IL_0024:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0029:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_002e:  nop
        IL_002f:  ret
      } // end of method '<>c__DisplayClass10_0'::'<Bug951b>b__0'

      .method assembly hidebysig instance void 
              '<Bug951b>b__1'() cil managed
      {
        // Code size       20 (0x14)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
        IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::NoOp(int32)
        IL_0012:  nop
        IL_0013:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
      } // end of method '<>c__DisplayClass11_0'::.ctor

      .method assembly hidebysig instance void 
              '<Bug951c>b__0'() cil managed
      {
        // Code size       26 (0x1a)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<Bug951c>b__1'()
        IL_000e:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
        IL_0018:  nop
        IL_0019:  ret
      } // end of method '<>c__DisplayClass11_0'::'<Bug951c>b__0'

      .method assembly hidebysig instance void 
              '<Bug951c>b__1'() cil managed
      {
        // Code size       25 (0x19)
        .maxstack  8
        IL_0000:  nop
        IL_0001:  ldarg.0
        IL_0002:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<>4__this'
        IL_0007:  ldarg.0
        IL_0008:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::data
        IL_000d:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData::Value
        IL_0012:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoSomething(string)
        IL_0017:  nop
        IL_0018:  ret
      } // end of method '<>c__DisplayClass11_0'::'<Bug951c>b__1'

    } // end of class '<>c__DisplayClass11_0'

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThis() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  2
      .locals init (class [mscorlib]System.Action V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<CaptureOfThis>b__1_0'()
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
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0' V_0,
               class [mscorlib]System.Action V_1)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::a
      IL_0014:  nop
      IL_0015:  ldloc.0
      IL_0016:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass2_0'::'<CaptureOfThisAndParameter>b__0'()
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
      // Code size       121 (0x79)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0' V_2,
               bool V_3,
               class [mscorlib]System.Action V_4)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1'::a
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_001b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0020:  stloc.1
      .try
      {
        IL_0021:  br.s       IL_005c

        IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::.ctor()
        IL_0028:  stloc.2
        IL_0029:  ldloc.2
        IL_002a:  ldloc.0
        IL_002b:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'CS$<>8__locals1'
        IL_0030:  ldloc.2
        IL_0031:  ldloc.1
        IL_0032:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::item
        IL_003c:  nop
        IL_003d:  ldloc.2
        IL_003e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::item
        IL_0043:  ldc.i4.0
        IL_0044:  cgt
        IL_0046:  stloc.3
        IL_0047:  ldloc.3
        IL_0048:  brfalse.s  IL_005b

        IL_004a:  nop
        IL_004b:  ldloc.2
        IL_004c:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass3_0'::'<CaptureOfThisAndParameterInForEach>b__0'()
        IL_0052:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_0057:  stloc.s    V_4
        IL_0059:  leave.s    IL_0076

        IL_005b:  nop
        IL_005c:  ldloc.1
        IL_005d:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0062:  brtrue.s   IL_0023

        IL_0064:  leave.s    IL_0071

      }  // end .try
      finally
      {
        IL_0066:  ldloc.1
        IL_0067:  brfalse.s  IL_0070

        IL_0069:  ldloc.1
        IL_006a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_006f:  nop
        IL_0070:  endfinally
      }  // end handler
      IL_0071:  ldnull
      IL_0072:  stloc.s    V_4
      IL_0074:  br.s       IL_0076

      IL_0076:  ldloc.s    V_4
      IL_0078:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEach

    .method public hidebysig instance class [mscorlib]System.Action 
            CaptureOfThisAndParameterInForEachWithItemCopy(int32 a) cil managed
    {
      // Code size       158 (0x9e)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' V_0,
               class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2' V_3,
               bool V_4,
               class [mscorlib]System.Action V_5)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::'<>4__this'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1'::a
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Empty<int32>()
      IL_001b:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_0020:  stloc.1
      .try
      {
        IL_0021:  br.s       IL_0081

        IL_0023:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::.ctor()
        IL_0028:  stloc.2
        IL_0029:  ldloc.2
        IL_002a:  ldloc.0
        IL_002b:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::'CS$<>8__locals1'
        IL_0030:  ldloc.2
        IL_0031:  ldloc.1
        IL_0032:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_003c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::.ctor()
        IL_0041:  stloc.3
        IL_0042:  ldloc.3
        IL_0043:  ldloc.2
        IL_0044:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0049:  nop
        IL_004a:  ldloc.3
        IL_004b:  ldloc.3
        IL_004c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0051:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_0056:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::copyOfItem
        IL_005b:  ldloc.3
        IL_005c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'CS$<>8__locals2'
        IL_0061:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_0'::item
        IL_0066:  ldc.i4.0
        IL_0067:  cgt
        IL_0069:  stloc.s    V_4
        IL_006b:  ldloc.s    V_4
        IL_006d:  brfalse.s  IL_0080

        IL_006f:  nop
        IL_0070:  ldloc.3
        IL_0071:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass4_2'::'<CaptureOfThisAndParameterInForEachWithItemCopy>b__0'()
        IL_0077:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                          native int)
        IL_007c:  stloc.s    V_5
        IL_007e:  leave.s    IL_009b

        IL_0080:  nop
        IL_0081:  ldloc.1
        IL_0082:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
        IL_0087:  brtrue.s   IL_0023

        IL_0089:  leave.s    IL_0096

      }  // end .try
      finally
      {
        IL_008b:  ldloc.1
        IL_008c:  brfalse.s  IL_0095

        IL_008e:  ldloc.1
        IL_008f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
        IL_0094:  nop
        IL_0095:  endfinally
      }  // end handler
      IL_0096:  ldnull
      IL_0097:  stloc.s    V_5
      IL_0099:  br.s       IL_009b

      IL_009b:  ldloc.s    V_5
      IL_009d:  ret
    } // end of method InstanceTests::CaptureOfThisAndParameterInForEachWithItemCopy

    .method public hidebysig instance void 
            LambdaInForLoop() cil managed
    {
      // Code size       43 (0x2b)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_001e

      IL_0005:  nop
      IL_0006:  ldarg.0
      IL_0007:  ldarg.0
      IL_0008:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::'<LambdaInForLoop>b__5_0'()
      IL_000e:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                     native int)
      IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Bar(class [mscorlib]System.Func`1<int32>)
      IL_0018:  nop
      IL_0019:  nop
      IL_001a:  ldloc.0
      IL_001b:  ldc.i4.1
      IL_001c:  add
      IL_001d:  stloc.0
      IL_001e:  ldloc.0
      IL_001f:  ldc.i4     0x186a0
      IL_0024:  clt
      IL_0026:  stloc.1
      IL_0027:  ldloc.1
      IL_0028:  brtrue.s   IL_0005

      IL_002a:  ret
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
      IL_0001:  ldsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__8_0'
      IL_0006:  dup
      IL_0007:  brtrue.s   IL_0020

      IL_0009:  pop
      IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9'
      IL_000f:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<Bug955>b__8_0'()
      IL_0015:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                       native int)
      IL_001a:  dup
      IL_001b:  stsfld     class [mscorlib]System.Threading.ThreadStart ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__8_0'
      IL_0020:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
      IL_0025:  pop
      IL_0026:  ret
    } // end of method InstanceTests::Bug955

    .method public hidebysig instance void 
            Bug951(int32 amount) cil managed
    {
      // Code size       41 (0x29)
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
      IL_0014:  nop
      IL_0015:  ldarg.0
      IL_0016:  ldloc.0
      IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass9_0'::'<Bug951>b__0'()
      IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0027:  nop
      IL_0028:  ret
    } // end of method InstanceTests::Bug951

    .method public hidebysig instance void 
            Bug951b() cil managed
    {
      // Code size       46 (0x2e)
      .maxstack  3
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<>4__this'
      IL_000d:  nop
      IL_000e:  ldloc.0
      IL_000f:  ldarg.0
      IL_0010:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::Foo()
      IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::amount
      IL_001a:  ldarg.0
      IL_001b:  ldloc.0
      IL_001c:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass10_0'::'<Bug951b>b__0'()
      IL_0022:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0027:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_002c:  nop
      IL_002d:  ret
    } // end of method InstanceTests::Bug951b

    .method public hidebysig instance void 
            Bug951c(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/SomeData data) cil managed
    {
      // Code size       41 (0x29)
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
      IL_0014:  nop
      IL_0015:  ldarg.0
      IL_0016:  ldloc.0
      IL_0017:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c__DisplayClass11_0'::'<Bug951c>b__0'()
      IL_001d:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                        native int)
      IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::DoAction(class [mscorlib]System.Action)
      IL_0027:  nop
      IL_0028:  ret
    } // end of method InstanceTests::Bug951c

    .method public hidebysig instance class [mscorlib]System.Action`1<object> 
            Bug971_DelegateWithoutParameterList() cil managed
    {
      // Code size       37 (0x25)
      .maxstack  2
      .locals init (class [mscorlib]System.Action`1<object> V_0)
      IL_0000:  nop
      IL_0001:  ldsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__12_0'
      IL_0006:  dup
      IL_0007:  brtrue.s   IL_0020

      IL_0009:  pop
      IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9'
      IL_000f:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<Bug971_DelegateWithoutParameterList>b__12_0'(object)
      IL_0015:  newobj     instance void class [mscorlib]System.Action`1<object>::.ctor(object,
                                                                                        native int)
      IL_001a:  dup
      IL_001b:  stsfld     class [mscorlib]System.Action`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests/'<>c'::'<>9__12_0'
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method InstanceTests::.ctor

    .method private hidebysig instance void 
            '<CaptureOfThis>b__1_0'() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  call       instance class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/InstanceTests::CaptureOfThis()
      IL_0007:  pop
      IL_0008:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass8_0'::.ctor

    .method assembly hidebysig instance void 
            '<AnonymousMethodStoreWithinLoop>b__0'(int32 x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0'::counter
      IL_0008:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass9_0'::.ctor

    .method assembly hidebysig instance void 
            '<AnonymousMethodStoreOutsideLoop>b__0'(int32 x) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::counter
      IL_0008:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c'::.ctor

    .method assembly hidebysig instance void 
            '<StaticAnonymousMethodNoClosure>b__10_0'() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  call       void [mscorlib]System.Console::WriteLine()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c'::'<StaticAnonymousMethodNoClosure>b__10_0'

    .method assembly hidebysig instance void 
            '<NameConflict2>b__12_0'(int32 i) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0007:  nop
      IL_0008:  ret
    } // end of method '<>c'::'<NameConflict2>b__12_0'

    .method assembly hidebysig instance void 
            '<NameConflict3>b__13_0'(int32 j) cil managed
    {
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
      IL_0013:  ldarg.1
      IL_0014:  clt
      IL_0016:  stloc.1
      IL_0017:  ldloc.1
      IL_0018:  brtrue.s   IL_0005

      IL_001a:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass11_0'::.ctor

    .method assembly hidebysig instance void 
            '<NameConflict>b__0'(int32 j) cil managed
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
      IL_0013:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
      IL_0018:  clt
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  brtrue.s   IL_0005

      IL_001e:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass14_0'::.ctor

    .method assembly hidebysig instance class [mscorlib]System.Func`2<int32,int32> 
            '<CurriedAddition>b__0'(int32 b) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::'CS$<>8__locals1'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::b
      IL_0014:  ldloc.0
      IL_0015:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_1'::'<CurriedAddition>b__1'(int32)
      IL_001b:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_0020:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass15_0'::.ctor

    .method assembly hidebysig instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> 
            '<CurriedAddition2>b__0'(int32 b) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::'CS$<>8__locals1'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::b
      IL_0014:  ldloc.0
      IL_0015:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1'::'<CurriedAddition2>b__1'(int32)
      IL_001b:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>::.ctor(object,
                                                                                                                                native int)
      IL_0020:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<>c__DisplayClass15_1'::.ctor

    .method assembly hidebysig instance class [mscorlib]System.Func`2<int32,int32> 
            '<CurriedAddition2>b__1'(int32 c) cil managed
    {
      // Code size       33 (0x21)
      .maxstack  2
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2' V_0)
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::.ctor()
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldarg.0
      IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::'CS$<>8__locals2'
      IL_000d:  ldloc.0
      IL_000e:  ldarg.1
      IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::c
      IL_0014:  ldloc.0
      IL_0015:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_2'::'<CurriedAddition2>b__2'(int32)
      IL_001b:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                           native int)
      IL_0020:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0' V_2,
             bool V_3,
             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_4)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_002a

    IL_000b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0'::.ctor()
    IL_0010:  stloc.2
    IL_0011:  nop
    IL_0012:  ldloc.0
    IL_0013:  ldloc.2
    IL_0014:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass8_0'::'<AnonymousMethodStoreWithinLoop>b__0'(int32)
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
    IL_002f:  stloc.3
    IL_0030:  ldloc.3
    IL_0031:  brtrue.s   IL_000b

    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_4
    IL_0036:  br.s       IL_0038

    IL_0038:  ldloc.s    V_4
    IL_003a:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreWithinLoop

  .method public hidebysig static class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> 
          AnonymousMethodStoreOutsideLoop() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0' V_0,
             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_1,
             int32 V_2,
             class [mscorlib]System.Action`1<int32> V_3,
             bool V_4,
             class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_5)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_003d

    IL_0011:  nop
    IL_0012:  ldloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldfld      class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::'<>9__0'
    IL_0019:  dup
    IL_001a:  brtrue.s   IL_0032

    IL_001c:  pop
    IL_001d:  ldloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::'<AnonymousMethodStoreOutsideLoop>b__0'(int32)
    IL_0025:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_002a:  dup
    IL_002b:  stloc.3
    IL_002c:  stfld      class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass9_0'::'<>9__0'
    IL_0031:  ldloc.3
    IL_0032:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldloc.2
    IL_003a:  ldc.i4.1
    IL_003b:  add
    IL_003c:  stloc.2
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.s   10
    IL_0040:  clt
    IL_0042:  stloc.s    V_4
    IL_0044:  ldloc.s    V_4
    IL_0046:  brtrue.s   IL_0011

    IL_0048:  ldloc.1
    IL_0049:  stloc.s    V_5
    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.s    V_5
    IL_004f:  ret
  } // end of method DelegateConstruction::AnonymousMethodStoreOutsideLoop

  .method public hidebysig static class [mscorlib]System.Action 
          StaticAnonymousMethodNoClosure() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (class [mscorlib]System.Action V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__10_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
    IL_000f:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<StaticAnonymousMethodNoClosure>b__10_0'()
    IL_0015:  newobj     instance void [mscorlib]System.Action::.ctor(object,
                                                                      native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Action ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__10_0'
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.0
    IL_0024:  ret
  } // end of method DelegateConstruction::StaticAnonymousMethodNoClosure

  .method public hidebysig static void  NameConflict() cil managed
  {
    // Code size       97 (0x61)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>> V_0,
             int32 V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0' V_2,
             int32 V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_0055

    IL_000b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::.ctor()
    IL_0010:  stloc.2
    IL_0011:  nop
    IL_0012:  ldloc.2
    IL_0013:  ldc.i4.0
    IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0019:  br.s       IL_0040

    IL_001b:  nop
    IL_001c:  ldloc.0
    IL_001d:  ldloc.2
    IL_001e:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::'<NameConflict>b__0'(int32)
    IL_0024:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0029:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Action`1<int32>>::Add(!0)
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  ldloc.2
    IL_0031:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0036:  stloc.3
    IL_0037:  ldloc.2
    IL_0038:  ldloc.3
    IL_0039:  ldc.i4.1
    IL_003a:  add
    IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0040:  ldloc.2
    IL_0041:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass11_0'::i
    IL_0046:  ldc.i4.s   10
    IL_0048:  clt
    IL_004a:  stloc.s    V_4
    IL_004c:  ldloc.s    V_4
    IL_004e:  brtrue.s   IL_001b

    IL_0050:  nop
    IL_0051:  ldloc.1
    IL_0052:  ldc.i4.1
    IL_0053:  add
    IL_0054:  stloc.1
    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.s   10
    IL_0058:  clt
    IL_005a:  stloc.s    V_5
    IL_005c:  ldloc.s    V_5
    IL_005e:  brtrue.s   IL_000b

    IL_0060:  ret
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
    IL_000d:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__12_0'
    IL_0012:  dup
    IL_0013:  brtrue.s   IL_002c

    IL_0015:  pop
    IL_0016:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
    IL_001b:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<NameConflict2>b__12_0'(int32)
    IL_0021:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_0026:  dup
    IL_0027:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__12_0'
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
    IL_0001:  ldsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__13_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9'
    IL_000f:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<NameConflict3>b__13_0'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Action`1<int32>::.ctor(object,
                                                                                     native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Action`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c'::'<>9__13_0'
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
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0' V_0,
             class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::a
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass14_0'::'<CurriedAddition>b__0'(int32)
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
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0' V_0,
             class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::a
    IL_000d:  nop
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction/'<>c__DisplayClass15_0'::'<CurriedAddition2>b__0'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.1
    IL_001e:  ret
  } // end of method DelegateConstruction::CurriedAddition2

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.DelegateConstruction


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
