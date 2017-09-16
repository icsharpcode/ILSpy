
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CheckedUnchecked
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
.module CheckedUnchecked.dll
// MVID: {BE44B64F-C3D4-4774-9952-97A7156C2BEB}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02440000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<x>j__TPar','<l>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<x>j__TPar' '<x>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<l>j__TPar' '<l>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0xf0268823
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<l>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType0`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<x>j__TPar' V_0,
             !'<x>j__TPar' V_1,
             !'<l>j__TPar' V_2,
             !'<l>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ x = {0}, l = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<x>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<x>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<x>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<x>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<x>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<x>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<x>j__TPar',!'<l>j__TPar'>::'<l>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<l>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<l>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<l>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<l>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<l>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .property instance !'<x>j__TPar' x()
  {
    .get instance !'<x>j__TPar' '<>f__AnonymousType0`2'::get_x()
  } // end of property '<>f__AnonymousType0`2'::x
  .property instance !'<l>j__TPar' l()
  {
    .get instance !'<l>j__TPar' '<>f__AnonymousType0`2'::get_l()
  } // end of property '<>f__AnonymousType0`2'::l
} // end of class '<>f__AnonymousType0`2'

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked
       extends [mscorlib]System.Object
{
  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> '<>9__4_0'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> '<>9__5_0'
    .field public static class [mscorlib]System.Func`2<int32[],int32[]> '<>9__6_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9'
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

    .method assembly hidebysig instance class '<>f__AnonymousType0`2'<int32,int32> 
            '<ObjectCreationInitializerChecked>b__4_0'(class '<>f__AnonymousType0`2'<int32,int32> n) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType0`2'<int32,int32>::get_x()
      IL_0006:  ldc.i4.1
      IL_0007:  add.ovf
      IL_0008:  ldarg.1
      IL_0009:  callvirt   instance !1 class '<>f__AnonymousType0`2'<int32,int32>::get_l()
      IL_000e:  ldc.i4.1
      IL_000f:  add.ovf
      IL_0010:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                           !1)
      IL_0015:  ret
    } // end of method '<>c'::'<ObjectCreationInitializerChecked>b__4_0'

    .method assembly hidebysig instance class '<>f__AnonymousType0`2'<int32,int32> 
            '<ObjectCreationWithOneFieldChecked>b__5_0'(class '<>f__AnonymousType0`2'<int32,int32> n) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType0`2'<int32,int32>::get_x()
      IL_0006:  ldc.i4.1
      IL_0007:  add.ovf
      IL_0008:  ldarg.1
      IL_0009:  callvirt   instance !1 class '<>f__AnonymousType0`2'<int32,int32>::get_l()
      IL_000e:  ldc.i4.1
      IL_000f:  add
      IL_0010:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                           !1)
      IL_0015:  ret
    } // end of method '<>c'::'<ObjectCreationWithOneFieldChecked>b__5_0'

    .method assembly hidebysig instance int32[] 
            '<ArrayInitializerChecked>b__6_0'(int32[] n) cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.2
      IL_0001:  newarr     [mscorlib]System.Int32
      IL_0006:  dup
      IL_0007:  ldc.i4.0
      IL_0008:  ldarg.1
      IL_0009:  ldc.i4.0
      IL_000a:  ldelem.i4
      IL_000b:  ldc.i4.1
      IL_000c:  add.ovf
      IL_000d:  stelem.i4
      IL_000e:  dup
      IL_000f:  ldc.i4.1
      IL_0010:  ldarg.1
      IL_0011:  ldc.i4.1
      IL_0012:  ldelem.i4
      IL_0013:  ldc.i4.1
      IL_0014:  add.ovf
      IL_0015:  stelem.i4
      IL_0016:  ret
    } // end of method '<>c'::'<ArrayInitializerChecked>b__6_0'

  } // end of class '<>c'

  .method public hidebysig instance int32 
          Operators(int32 a,
                    int32 b) cil managed
  {
    // Code size       52 (0x34)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             int32 V_6)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  add.ovf
    IL_0003:  ldarg.1
    IL_0004:  ldarg.2
    IL_0005:  add
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldarg.2
    IL_0009:  sub.ovf
    IL_000a:  stloc.1
    IL_000b:  ldarg.1
    IL_000c:  ldarg.2
    IL_000d:  sub
    IL_000e:  stloc.2
    IL_000f:  ldarg.1
    IL_0010:  ldarg.2
    IL_0011:  mul.ovf
    IL_0012:  stloc.3
    IL_0013:  ldarg.1
    IL_0014:  ldarg.2
    IL_0015:  mul
    IL_0016:  stloc.s    V_4
    IL_0018:  ldarg.1
    IL_0019:  ldarg.2
    IL_001a:  div
    IL_001b:  stloc.s    V_5
    IL_001d:  ldarg.1
    IL_001e:  ldarg.2
    IL_001f:  rem
    IL_0020:  stloc.s    V_6
    IL_0022:  ldloc.0
    IL_0023:  mul
    IL_0024:  ldloc.1
    IL_0025:  mul
    IL_0026:  ldloc.2
    IL_0027:  mul
    IL_0028:  ldloc.3
    IL_0029:  mul
    IL_002a:  ldloc.s    V_4
    IL_002c:  mul
    IL_002d:  ldloc.s    V_5
    IL_002f:  mul
    IL_0030:  ldloc.s    V_6
    IL_0032:  mul
    IL_0033:  ret
  } // end of method CheckedUnchecked::Operators

  .method public hidebysig instance int32 
          Cast(int32 a) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (int16 V_0,
             uint8 V_1,
             uint8 V_2)
    IL_0000:  ldarg.1
    IL_0001:  conv.ovf.i2
    IL_0002:  ldarg.1
    IL_0003:  conv.i2
    IL_0004:  stloc.0
    IL_0005:  ldarg.1
    IL_0006:  conv.ovf.u1
    IL_0007:  stloc.1
    IL_0008:  ldarg.1
    IL_0009:  conv.u1
    IL_000a:  stloc.2
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  ldloc.1
    IL_000e:  mul
    IL_000f:  ldloc.2
    IL_0010:  mul
    IL_0011:  ret
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
    // Code size       46 (0x2e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9__4_0'
    IL_000d:  dup
    IL_000e:  brtrue.s   IL_0027

    IL_0010:  pop
    IL_0011:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9'
    IL_0016:  ldftn      instance class '<>f__AnonymousType0`2'<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<ObjectCreationInitializerChecked>b__4_0'(class '<>f__AnonymousType0`2'<int32,int32>)
    IL_001c:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_0021:  dup
    IL_0022:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9__4_0'
    IL_0027:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<class '<>f__AnonymousType0`2'<int32,int32>>(!!0,
                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!0>)
    IL_002c:  pop
    IL_002d:  ret
  } // end of method CheckedUnchecked::ObjectCreationInitializerChecked

  .method public hidebysig instance void 
          ObjectCreationWithOneFieldChecked() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void class '<>f__AnonymousType0`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9__5_0'
    IL_000d:  dup
    IL_000e:  brtrue.s   IL_0027

    IL_0010:  pop
    IL_0011:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9'
    IL_0016:  ldftn      instance class '<>f__AnonymousType0`2'<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<ObjectCreationWithOneFieldChecked>b__5_0'(class '<>f__AnonymousType0`2'<int32,int32>)
    IL_001c:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>>::.ctor(object,
                                                                                                                                                                   native int)
    IL_0021:  dup
    IL_0022:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType0`2'<int32,int32>,class '<>f__AnonymousType0`2'<int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9__5_0'
    IL_0027:  call       instance !!0 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked::TestHelp<class '<>f__AnonymousType0`2'<int32,int32>>(!!0,
                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!0>)
    IL_002c:  pop
    IL_002d:  ret
  } // end of method CheckedUnchecked::ObjectCreationWithOneFieldChecked

  .method public hidebysig instance void 
          ArrayInitializerChecked() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.1
    IL_000a:  stelem.i4
    IL_000b:  dup
    IL_000c:  ldc.i4.1
    IL_000d:  ldc.i4.2
    IL_000e:  stelem.i4
    IL_000f:  ldsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9__6_0'
    IL_0014:  dup
    IL_0015:  brtrue.s   IL_002e

    IL_0017:  pop
    IL_0018:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9'
    IL_001d:  ldftn      instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<ArrayInitializerChecked>b__6_0'(int32[])
    IL_0023:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32[]>::.ctor(object,
                                                                                             native int)
    IL_0028:  dup
    IL_0029:  stsfld     class [mscorlib]System.Func`2<int32[],int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked/'<>c'::'<>9__6_0'
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

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CheckedUnchecked


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
