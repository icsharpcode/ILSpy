
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly j4foxgdl
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module j4foxgdl.dll
// MVID: {C884AA00-D1B2-4593-9A69-3DBFE92E25C4}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02F90000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked
       extends [mscorlib]System.Object
{
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> 'CS$<>9__CachedAnonymousMethodDelegate1'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> 'CS$<>9__CachedAnonymousMethodDelegate3'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<int32[],int32[]> 'CS$<>9__CachedAnonymousMethodDelegate5'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig instance int32 
          Operators(int32 a,
                    int32 b) cil managed
  {
    // Code size       56 (0x38)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             int32 V_7)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  add.ovf
    IL_0003:  stloc.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  add
    IL_0007:  stloc.1
    IL_0008:  ldarg.1
    IL_0009:  ldarg.2
    IL_000a:  sub.ovf
    IL_000b:  stloc.2
    IL_000c:  ldarg.1
    IL_000d:  ldarg.2
    IL_000e:  sub
    IL_000f:  stloc.3
    IL_0010:  ldarg.1
    IL_0011:  ldarg.2
    IL_0012:  mul.ovf
    IL_0013:  stloc.s    V_4
    IL_0015:  ldarg.1
    IL_0016:  ldarg.2
    IL_0017:  mul
    IL_0018:  stloc.s    V_5
    IL_001a:  ldarg.1
    IL_001b:  ldarg.2
    IL_001c:  div
    IL_001d:  stloc.s    V_6
    IL_001f:  ldarg.1
    IL_0020:  ldarg.2
    IL_0021:  rem
    IL_0022:  stloc.s    V_7
    IL_0024:  ldloc.0
    IL_0025:  ldloc.1
    IL_0026:  mul
    IL_0027:  ldloc.2
    IL_0028:  mul
    IL_0029:  ldloc.3
    IL_002a:  mul
    IL_002b:  ldloc.s    V_4
    IL_002d:  mul
    IL_002e:  ldloc.s    V_5
    IL_0030:  mul
    IL_0031:  ldloc.s    V_6
    IL_0033:  mul
    IL_0034:  ldloc.s    V_7
    IL_0036:  mul
    IL_0037:  ret
  } // end of method CheckedUnchecked::Operators

  .method public hidebysig instance int32 
          Cast(int32 a) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (int16 V_0,
             int16 V_1,
             uint8 V_2,
             uint8 V_3)
    IL_0000:  ldarg.1
    IL_0001:  conv.ovf.i2
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  conv.i2
    IL_0005:  stloc.1
    IL_0006:  ldarg.1
    IL_0007:  conv.ovf.u1
    IL_0008:  stloc.2
    IL_0009:  ldarg.1
    IL_000a:  conv.u1
    IL_000b:  stloc.3
    IL_000c:  ldloc.0
    IL_000d:  ldloc.1
    IL_000e:  mul
    IL_000f:  ldloc.2
    IL_0010:  mul
    IL_0011:  ldloc.3
    IL_0012:  mul
    IL_0013:  ret
  } // end of method CheckedUnchecked::Cast

  .method public hidebysig instance void 
          ForWithCheckedIteratorAndUncheckedBody(int32 n) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  add.ovf
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_000f

    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  mul
    IL_0009:  starg.s    n
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  add.ovf
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.1
    IL_0012:  add.ovf
    IL_0013:  blt.s      IL_0006

    IL_0015:  ret
  } // end of method CheckedUnchecked::ForWithCheckedIteratorAndUncheckedBody

  .method public hidebysig instance void 
          ForWithCheckedInitializerAndUncheckedIterator(int32 n) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  sub.ovf
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0012

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  starg.s    n
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  ldarg.1
    IL_0014:  blt.s      IL_0009

    IL_0016:  ret
  } // end of method CheckedUnchecked::ForWithCheckedInitializerAndUncheckedIterator

  .method public hidebysig instance void 
          ObjectCreationInitializerChecked() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_000d:  brtrue.s   IL_0020

    IL_000f:  ldnull
    IL_0010:  ldftn      class '<>f__AnonymousType0`2'<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'<ObjectCreationInitializerChecked>b__0'(class '<>f__AnonymousType0`2'<int32,int32>)
    IL_0016:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_001b:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_0020:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_0025:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<class '<>f__AnonymousType0`2'<int32,int32>>(!!0,
                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!0>)
    IL_002a:  pop
    IL_002b:  ret
  } // end of method CheckedUnchecked::ObjectCreationInitializerChecked

  .method public hidebysig instance void 
          ObjectCreationWithOneFieldChecked() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_000d:  brtrue.s   IL_0020

    IL_000f:  ldnull
    IL_0010:  ldftn      class '<>f__AnonymousType0`2'<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'<ObjectCreationWithOneFieldChecked>b__2'(class '<>f__AnonymousType0`2'<int32,int32>)
    IL_0016:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_001b:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0020:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0025:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<class '<>f__AnonymousType0`2'<int32,int32>>(!!0,
                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!0>)
    IL_002a:  pop
    IL_002b:  ret
  } // end of method CheckedUnchecked::ObjectCreationWithOneFieldChecked

  .method public hidebysig instance void 
          ArrayInitializerChecked() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.1
    IL_000b:  stelem.i4
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  ldc.i4.2
    IL_000f:  stelem.i4
    IL_0010:  ldloc.0
    IL_0011:  ldsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate5'
    IL_0016:  brtrue.s   IL_0029

    IL_0018:  ldnull
    IL_0019:  ldftn      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'<ArrayInitializerChecked>b__4'(int32[])
    IL_001f:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32[]>::.ctor(object,
                                                                                             native int)
    IL_0024:  stsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate5'
    IL_0029:  ldsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate5'
    IL_002e:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<int32[]>(!!0,
                                                                                                                        class [mscorlib]System.Func`2<!!0,!!0>)
    IL_0033:  pop
    IL_0034:  ret
  } // end of method CheckedUnchecked::ArrayInitializerChecked

  .method public hidebysig instance !!T  TestHelp<T>(!!T t,
                                                     class [mscorlib]System.Func`2<!!T,!!T> f) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance !1 class [mscorlib]System.Func`2<!!T,!!T>::Invoke(!0)
    IL_0007:  ret
  } // end of method CheckedUnchecked::TestHelp

  .method public hidebysig instance void 
          CheckedInArrayCreationArgument(int32 a,
                                         int32 b) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  add.ovf
    IL_0003:  newarr     [mscorlib]System.Int32
    IL_0008:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_000d:  ret
  } // end of method CheckedUnchecked::CheckedInArrayCreationArgument

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method CheckedUnchecked::.ctor

  .method private hidebysig static class '<>f__AnonymousType0`2'<int32,int32> 
          '<ObjectCreationInitializerChecked>b__0'(class '<>f__AnonymousType0`2'<int32,int32> n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType0`2'<int32,int32>::get_x()
    IL_0006:  ldc.i4.1
    IL_0007:  add.ovf
    IL_0008:  ldarg.0
    IL_0009:  callvirt   instance !1 class '<>f__AnonymousType0`2'<int32,int32>::get_l()
    IL_000e:  ldc.i4.1
    IL_000f:  add.ovf
    IL_0010:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0015:  ret
  } // end of method CheckedUnchecked::'<ObjectCreationInitializerChecked>b__0'

  .method private hidebysig static class '<>f__AnonymousType0`2'<int32,int32> 
          '<ObjectCreationWithOneFieldChecked>b__2'(class '<>f__AnonymousType0`2'<int32,int32> n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType0`2'<int32,int32>::get_x()
    IL_0006:  ldc.i4.1
    IL_0007:  add.ovf
    IL_0008:  ldarg.0
    IL_0009:  callvirt   instance !1 class '<>f__AnonymousType0`2'<int32,int32>::get_l()
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0015:  ret
  } // end of method CheckedUnchecked::'<ObjectCreationWithOneFieldChecked>b__2'

  .method private hidebysig static int32[] 
          '<ArrayInitializerChecked>b__4'(int32[] n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       25 (0x19)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.2
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldelem.i4
    IL_000c:  ldc.i4.1
    IL_000d:  add.ovf
    IL_000e:  stelem.i4
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.1
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.1
    IL_0013:  ldelem.i4
    IL_0014:  ldc.i4.1
    IL_0015:  add.ovf
    IL_0016:  stelem.i4
    IL_0017:  ldloc.0
    IL_0018:  ret
  } // end of method CheckedUnchecked::'<ArrayInitializerChecked>b__4'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<x>j__TPar','<l>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<x>j__TPar' '<x>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<l>j__TPar' '<l>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<x>j__TPar' x,
                               !'<l>j__TPar' l) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType0`2'::.ctor

  .method public hidebysig specialname instance !'<x>j__TPar' 
          get_x() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_x

  .method public hidebysig specialname instance !'<l>j__TPar' 
          get_l() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_l

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ x = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0019:  box        !'<x>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", l = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0037:  box        !'<l>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType0`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xf749ae7d
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType0`2'::GetHashCode

  .property instance !'<x>j__TPar' x()
  {
    .get instance !'<x>j__TPar' '<>f__AnonymousType0`2'::get_x()
  } // end of property '<>f__AnonymousType0`2'::x
  .property instance !'<l>j__TPar' l()
  {
    .get instance !'<l>j__TPar' '<>f__AnonymousType0`2'::get_l()
  } // end of property '<>f__AnonymousType0`2'::l
} // end of class '<>f__AnonymousType0`2'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\CheckedUnchecked.opt.res
