
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly rbj4xpgi
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module rbj4xpgi.dll
// MVID: {45787808-282D-44B8-BFE5-F8F3EF6DDA83}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x005F0000


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
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  add.ovf
    IL_0004:  stloc.0
    IL_0005:  ldarg.1
    IL_0006:  ldarg.2
    IL_0007:  add
    IL_0008:  stloc.1
    IL_0009:  ldarg.1
    IL_000a:  ldarg.2
    IL_000b:  sub.ovf
    IL_000c:  stloc.2
    IL_000d:  ldarg.1
    IL_000e:  ldarg.2
    IL_000f:  sub
    IL_0010:  stloc.3
    IL_0011:  ldarg.1
    IL_0012:  ldarg.2
    IL_0013:  mul.ovf
    IL_0014:  stloc.s    V_4
    IL_0016:  ldarg.1
    IL_0017:  ldarg.2
    IL_0018:  mul
    IL_0019:  stloc.s    V_5
    IL_001b:  ldarg.1
    IL_001c:  ldarg.2
    IL_001d:  div
    IL_001e:  stloc.s    V_6
    IL_0020:  ldarg.1
    IL_0021:  ldarg.2
    IL_0022:  rem
    IL_0023:  stloc.s    V_7
    IL_0025:  ldloc.0
    IL_0026:  ldloc.1
    IL_0027:  mul
    IL_0028:  ldloc.2
    IL_0029:  mul
    IL_002a:  ldloc.3
    IL_002b:  mul
    IL_002c:  ldloc.s    V_4
    IL_002e:  mul
    IL_002f:  ldloc.s    V_5
    IL_0031:  mul
    IL_0032:  ldloc.s    V_6
    IL_0034:  mul
    IL_0035:  ldloc.s    V_7
    IL_0037:  mul
    IL_0038:  stloc.s    V_8
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.s    V_8
    IL_003e:  ret
  } // end of method CheckedUnchecked::Operators

  .method public hidebysig instance int32 
          Cast(int32 a) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (int16 V_0,
             int16 V_1,
             uint8 V_2,
             uint8 V_3,
             int32 V_4)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.ovf.i2
    IL_0003:  stloc.0
    IL_0004:  ldarg.1
    IL_0005:  conv.i2
    IL_0006:  stloc.1
    IL_0007:  ldarg.1
    IL_0008:  conv.ovf.u1
    IL_0009:  stloc.2
    IL_000a:  ldarg.1
    IL_000b:  conv.u1
    IL_000c:  stloc.3
    IL_000d:  ldloc.0
    IL_000e:  ldloc.1
    IL_000f:  mul
    IL_0010:  ldloc.2
    IL_0011:  mul
    IL_0012:  ldloc.3
    IL_0013:  mul
    IL_0014:  stloc.s    V_4
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.s    V_4
    IL_001a:  ret
  } // end of method CheckedUnchecked::Cast

  .method public hidebysig instance void 
          ForWithCheckedIteratorAndUncheckedBody(int32 n) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  3
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.1
    IL_0003:  ldc.i4.1
    IL_0004:  add.ovf
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0013

    IL_0008:  nop
    IL_0009:  ldloc.0
    IL_000a:  ldloc.0
    IL_000b:  mul
    IL_000c:  starg.s    n
    IL_000e:  nop
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.1
    IL_0011:  add.ovf
    IL_0012:  stloc.0
    IL_0013:  ldloc.0
    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.1
    IL_0016:  add.ovf
    IL_0017:  clt
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  brtrue.s   IL_0008

    IL_001d:  nop
    IL_001e:  ret
  } // end of method CheckedUnchecked::ForWithCheckedIteratorAndUncheckedBody

  .method public hidebysig instance void 
          ForWithCheckedInitializerAndUncheckedIterator(int32 n) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   10
    IL_0006:  sub.ovf
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_0015

    IL_000a:  nop
    IL_000b:  ldarg.1
    IL_000c:  ldc.i4.1
    IL_000d:  sub
    IL_000e:  starg.s    n
    IL_0010:  nop
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldarg.1
    IL_0017:  clt
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  brtrue.s   IL_000a

    IL_001d:  ret
  } // end of method CheckedUnchecked::ForWithCheckedInitializerAndUncheckedIterator

  .method public hidebysig instance void 
          ObjectCreationInitializerChecked() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0009:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_000e:  brtrue.s   IL_0023

    IL_0010:  ldnull
    IL_0011:  ldftn      class '<>f__AnonymousType0`2'<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'<ObjectCreationInitializerChecked>b__0'(class '<>f__AnonymousType0`2'<int32,int32>)
    IL_0017:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_001c:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_0021:  br.s       IL_0023

    IL_0023:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate1'
    IL_0028:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<class '<>f__AnonymousType0`2'<int32,int32>>(!!0,
                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!0>)
    IL_002d:  pop
    IL_002e:  ret
  } // end of method CheckedUnchecked::ObjectCreationInitializerChecked

  .method public hidebysig instance void 
          ObjectCreationWithOneFieldChecked() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0009:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_000e:  brtrue.s   IL_0023

    IL_0010:  ldnull
    IL_0011:  ldftn      class '<>f__AnonymousType0`2'<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'<ObjectCreationWithOneFieldChecked>b__2'(class '<>f__AnonymousType0`2'<int32,int32>)
    IL_0017:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_001c:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0021:  br.s       IL_0023

    IL_0023:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0028:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<class '<>f__AnonymousType0`2'<int32,int32>>(!!0,
                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!0>)
    IL_002d:  pop
    IL_002e:  ret
  } // end of method CheckedUnchecked::ObjectCreationWithOneFieldChecked

  .method public hidebysig instance void 
          ArrayInitializerChecked() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.2
    IL_0003:  newarr     [mscorlib]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.1
    IL_000c:  stelem.i4
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  ldc.i4.2
    IL_0010:  stelem.i4
    IL_0011:  ldloc.0
    IL_0012:  ldsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate5'
    IL_0017:  brtrue.s   IL_002c

    IL_0019:  ldnull
    IL_001a:  ldftn      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'<ArrayInitializerChecked>b__4'(int32[])
    IL_0020:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32[]>::.ctor(object,
                                                                                             native int)
    IL_0025:  stsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate5'
    IL_002a:  br.s       IL_002c

    IL_002c:  ldsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::'CS$<>9__CachedAnonymousMethodDelegate5'
    IL_0031:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<int32[]>(!!0,
                                                                                                                        class [mscorlib]System.Func`2<!!0,!!0>)
    IL_0036:  pop
    IL_0037:  ret
  } // end of method CheckedUnchecked::ArrayInitializerChecked

  .method public hidebysig instance !!T  TestHelp<T>(!!T t,
                                                     class [mscorlib]System.Func`2<!!T,!!T> f) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<!!T,!!T>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method CheckedUnchecked::TestHelp

  .method public hidebysig instance void 
          CheckedInArrayCreationArgument(int32 a,
                                         int32 b) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  add.ovf
    IL_0004:  newarr     [mscorlib]System.Int32
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_000e:  nop
    IL_000f:  ret
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
    // Code size       26 (0x1a)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<int32,int32> V_0)
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
    IL_0015:  stloc.0
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method CheckedUnchecked::'<ObjectCreationInitializerChecked>b__0'

  .method private hidebysig static class '<>f__AnonymousType0`2'<int32,int32> 
          '<ObjectCreationWithOneFieldChecked>b__2'(class '<>f__AnonymousType0`2'<int32,int32> n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       26 (0x1a)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<int32,int32> V_0)
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
    IL_0015:  stloc.0
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method CheckedUnchecked::'<ObjectCreationWithOneFieldChecked>b__2'

  .method private hidebysig static int32[] 
          '<ArrayInitializerChecked>b__4'(int32[] n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int32[] V_0,
             int32[] V_1)
    IL_0000:  ldc.i4.2
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldelem.i4
    IL_000c:  ldc.i4.1
    IL_000d:  add.ovf
    IL_000e:  stelem.i4
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.1
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.1
    IL_0013:  ldelem.i4
    IL_0014:  ldc.i4.1
    IL_0015:  add.ovf
    IL_0016:  stelem.i4
    IL_0017:  ldloc.1
    IL_0018:  stloc.0
    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.0
    IL_001c:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<x>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType0`2'::get_x

  .method public hidebysig specialname instance !'<l>j__TPar' 
          get_l() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<l>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType0`2'::get_l

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
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
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'> V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType0`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
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
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\CheckedUnchecked.res
