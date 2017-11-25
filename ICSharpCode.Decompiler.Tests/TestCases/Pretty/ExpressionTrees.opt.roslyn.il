
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
.assembly extern System.Xml
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ExpressionTrees
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
.module ExpressionTrees.dll
// MVID: {E7B76CAF-AEAC-4B94-A679-C6A5DAD2815C}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x048B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<X>j__TPar','<A>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<A>j__TPar' '<A>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_X

  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_A

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<A>j__TPar' A) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType0`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
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
    IL_0000:  ldc.i4     0x8fa3881e
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType0`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<X>j__TPar' V_0,
             !'<X>j__TPar' V_1,
             !'<A>j__TPar' V_2,
             !'<A>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ X = {0}, A = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<X>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<X>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<X>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<X>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<X>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<A>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<A>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<A>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<A>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<A>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType0`2'::get_X()
  } // end of property '<>f__AnonymousType0`2'::X
  .property instance !'<A>j__TPar' A()
  {
    .get instance !'<A>j__TPar' '<>f__AnonymousType0`2'::get_A()
  } // end of property '<>f__AnonymousType0`2'::A
} // end of class '<>f__AnonymousType0`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`2'<'<X>j__TPar','<Y>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Y>j__TPar' '<Y>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_Y

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<Y>j__TPar' Y) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType1`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType1`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0xe71b881e
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType1`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<X>j__TPar' V_0,
             !'<X>j__TPar' V_1,
             !'<Y>j__TPar' V_2,
             !'<Y>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ X = {0}, Y = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<X>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<X>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<X>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<X>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<X>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<Y>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<Y>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<Y>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<Y>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<Y>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType1`2'::get_X()
  } // end of property '<>f__AnonymousType1`2'::X
  .property instance !'<Y>j__TPar' Y()
  {
    .get instance !'<Y>j__TPar' '<>f__AnonymousType1`2'::get_Y()
  } // end of property '<>f__AnonymousType1`2'::Y
} // end of class '<>f__AnonymousType1`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType2`2'<'<A>j__TPar','<B>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<A>j__TPar' '<A>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<B>j__TPar' '<B>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`2'::get_A

  .method public hidebysig specialname instance !'<B>j__TPar' 
          get_B() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`2'::get_B

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<A>j__TPar' A,
                               !'<B>j__TPar' B) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType2`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType2`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x62ad881e
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType2`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<A>j__TPar' V_0,
             !'<A>j__TPar' V_1,
             !'<B>j__TPar' V_2,
             !'<B>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ A = {0}, B = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<A>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<A>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<A>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<A>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<A>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<B>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<B>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<B>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<B>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<B>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType2`2'::ToString

  .property instance !'<A>j__TPar' A()
  {
    .get instance !'<A>j__TPar' '<>f__AnonymousType2`2'::get_A()
  } // end of property '<>f__AnonymousType2`2'::A
  .property instance !'<B>j__TPar' B()
  {
    .get instance !'<B>j__TPar' '<>f__AnonymousType2`2'::get_B()
  } // end of property '<>f__AnonymousType2`2'::B
} // end of class '<>f__AnonymousType2`2'

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit GenericClass`1<X>
         extends [mscorlib]System.Object
  {
    .field public static !X StaticField
    .field public !X InstanceField
    .field private static !X '<StaticProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private !X '<InstanceProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname static 
            !X  get_StaticProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0005:  ret
    } // end of method GenericClass`1::get_StaticProperty

    .method public hidebysig specialname static 
            void  set_StaticProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  stsfld     !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::set_StaticProperty

    .method public hidebysig specialname 
            instance !X  get_InstanceProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::get_InstanceProperty

    .method public hidebysig specialname 
            instance void  set_InstanceProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0007:  ret
    } // end of method GenericClass`1::set_InstanceProperty

    .method public hidebysig static bool 
            GenericMethod<Y>() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method GenericClass`1::GenericMethod

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method GenericClass`1::.ctor

    .property !X StaticProperty()
    {
      .get !X ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::get_StaticProperty()
      .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::set_StaticProperty(!X)
    } // end of property GenericClass`1::StaticProperty
    .property instance !X InstanceProperty()
    {
      .get instance !X ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::get_InstanceProperty()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1::set_InstanceProperty(!X)
    } // end of property GenericClass`1::InstanceProperty
  } // end of class GenericClass`1

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass7_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass7_0'::.ctor

  } // end of class '<>c__DisplayClass7_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass8_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass8_0'::.ctor

  } // end of class '<>c__DisplayClass8_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass10_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass10_0'::.ctor

  } // end of class '<>c__DisplayClass10_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass18_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> dict
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass18_0'::.ctor

  } // end of class '<>c__DisplayClass18_0'

  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<int32,string> '<>9__18_0'
    .field public static class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> '<>9__33_0'
    .field public static class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> '<>9__36_0'
    .field public static class [mscorlib]System.Func`2<int32[],int32> '<>9__62_0'
    .field public static class [mscorlib]System.Func`3<int32[],int32,int32> '<>9__62_1'
    .field public static class [mscorlib]System.Func`2<int32[0...,0...],int32> '<>9__62_2'
    .field public static class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> '<>9__62_3'
    .field public static class [mscorlib]System.Func`3<int32[][],int32,int32> '<>9__62_4'
    .field public static class [mscorlib]System.Func`2<int32[],int32> '<>9__63_0'
    .field public static class [mscorlib]System.Func`1<int32> '<>9__63_1'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_0'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_1'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_2'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_3'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_4'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_5'
    .field public static class [mscorlib]System.Func`1<object> '<>9__64_6'
    .field public static class [mscorlib]System.Func`1<class [mscorlib]System.Type> '<>9__65_0'
    .field public static class [mscorlib]System.Func`1<class [mscorlib]System.Type> '<>9__65_1'
    .field public static class [mscorlib]System.Func`1<class [mscorlib]System.Type> '<>9__65_2'
    .field public static class [mscorlib]System.Func`1<class [mscorlib]System.Type> '<>9__65_3'
    .field public static class [mscorlib]System.Func`1<class [mscorlib]System.Type> '<>9__65_4'
    .field public static class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> '<>9__66_0'
    .field public static class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> '<>9__66_1'
    .field public static class [mscorlib]System.Func`2<object,bool> '<>9__67_0'
    .field public static class [mscorlib]System.Func`2<bool,bool> '<>9__68_0'
    .field public static class [mscorlib]System.Func`2<int32,int32> '<>9__71_0'
    .field public static class [mscorlib]System.Func`2<int32,int32> '<>9__71_1'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__72_0'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__72_1'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__72_2'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__72_3'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__72_4'
    .field public static class [mscorlib]System.Func`3<int64,int32,int64> '<>9__72_5'
    .field public static class [mscorlib]System.Func`3<int64,int32,int64> '<>9__72_6'
    .field public static class [mscorlib]System.Func`3<int64,int32,int64> '<>9__72_7'
    .field public static class [mscorlib]System.Func`3<int64,int32,int64> '<>9__72_8'
    .field public static class [mscorlib]System.Func`3<int64,int32,int64> '<>9__72_9'
    .field public static class [mscorlib]System.Func`3<int16,int32,int32> '<>9__72_10'
    .field public static class [mscorlib]System.Func`3<int32,int16,int32> '<>9__72_11'
    .field public static class [mscorlib]System.Func`3<int16,int32,int32> '<>9__72_12'
    .field public static class [mscorlib]System.Func`3<int32,int16,int32> '<>9__72_13'
    .field public static class [mscorlib]System.Func`3<int16,int32,int32> '<>9__72_14'
    .field public static class [mscorlib]System.Func`2<int32,int32> '<>9__73_0'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__73_1'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__73_2'
    .field public static class [mscorlib]System.Func`3<int32,int32,int32> '<>9__73_3'
    .field public static class [mscorlib]System.Func`2<int32,int32> '<>9__74_0'
    .field public static class [mscorlib]System.Func`2<int32,int32> '<>9__74_1'
    .field public static class [mscorlib]System.Func`2<int64,int64> '<>9__74_2'
    .field public static class [mscorlib]System.Func`2<int64,int64> '<>9__74_3'
    .field public static class [mscorlib]System.Func`1<int32> '<>9__75_0'
    .field public static class [mscorlib]System.Func`2<int32,int32> '<>9__75_1'
    .field public static class [mscorlib]System.Func`2<string,string> '<>9__78_0'
    .field public static class [mscorlib]System.Func`2<int32,string> '<>9__78_1'
    .field public static class [mscorlib]System.Func`2<string,char[]> '<>9__78_2'
    .field public static class [mscorlib]System.Func`1<bool> '<>9__78_3'
    .field public static class [mscorlib]System.Func`1<bool> '<>9__79_0'
    .field public static class [mscorlib]System.Func`1<int32[]> '<>9__80_0'
    .field public static class [mscorlib]System.Func`1<int32[]> '<>9__80_1'
    .field public static class [mscorlib]System.Func`1<int32[0...,0...]> '<>9__80_2'
    .field public static class [mscorlib]System.Func`1<int32[][]> '<>9__80_3'
    .field public static class [mscorlib]System.Func`1<int32[][]> '<>9__80_4'
    .field public static class [mscorlib]System.Func`1<object> '<>9__81_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
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

    .method assembly hidebysig instance string 
            '<IndexerAccess>b__18_0'(int32 n) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarga.s   n
      IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
      IL_0007:  ret
    } // end of method '<>c'::'<IndexerAccess>b__18_0'

    .method assembly hidebysig instance bool 
            '<MethodGroupConstant>b__33_0'(class [mscorlib]System.Func`3<object,object,bool> f) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  callvirt   instance !2 class [mscorlib]System.Func`3<object,object,bool>::Invoke(!0,
                                                                                                 !1)
      IL_0008:  ret
    } // end of method '<>c'::'<MethodGroupConstant>b__33_0'

    .method assembly hidebysig instance int32 
            '<NestedLambda>b__36_0'(class [mscorlib]System.Func`1<int32> f) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
      IL_0006:  ret
    } // end of method '<>c'::'<NestedLambda>b__36_0'

    .method assembly hidebysig instance int32 
            '<ArrayIndexer>b__62_0'(int32[] 'array') cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.0
      IL_0002:  ldelem.i4
      IL_0003:  ret
    } // end of method '<>c'::'<ArrayIndexer>b__62_0'

    .method assembly hidebysig instance int32 
            '<ArrayIndexer>b__62_1'(int32[] 'array',
                                    int32 index) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  ldelem.i4
      IL_0003:  ret
    } // end of method '<>c'::'<ArrayIndexer>b__62_1'

    .method assembly hidebysig instance int32 
            '<ArrayIndexer>b__62_2'(int32[0...,0...] 'array') cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.5
      IL_0003:  call       instance int32 int32[0...,0...]::Get(int32,
                                                                int32)
      IL_0008:  ret
    } // end of method '<>c'::'<ArrayIndexer>b__62_2'

    .method assembly hidebysig instance int32 
            '<ArrayIndexer>b__62_3'(int32[0...,0...] 'array',
                                    int32 index) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  ldc.i4.7
      IL_0003:  call       instance int32 int32[0...,0...]::Get(int32,
                                                                int32)
      IL_0008:  ret
    } // end of method '<>c'::'<ArrayIndexer>b__62_3'

    .method assembly hidebysig instance int32 
            '<ArrayIndexer>b__62_4'(int32[][] 'array',
                                    int32 index) cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  ldelem.ref
      IL_0003:  ldc.i4.7
      IL_0004:  ldelem.i4
      IL_0005:  ret
    } // end of method '<>c'::'<ArrayIndexer>b__62_4'

    .method assembly hidebysig instance int32 
            '<ArrayLength>b__63_0'(int32[] 'array') cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldlen
      IL_0002:  conv.i4
      IL_0003:  ret
    } // end of method '<>c'::'<ArrayLength>b__63_0'

    .method assembly hidebysig instance int32 
            '<ArrayLength>b__63_1'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  callvirt   instance int32 [mscorlib]System.Array::get_Length()
      IL_0006:  ret
    } // end of method '<>c'::'<ArrayLength>b__63_1'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_0'() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::.ctor()
      IL_0005:  ret
    } // end of method '<>c'::'<NewObj>b__64_0'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_1'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldc.i4.5
      IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
      IL_0006:  ret
    } // end of method '<>c'::'<NewObj>b__64_1'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_2'() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor()
      IL_0005:  ret
    } // end of method '<>c'::'<NewObj>b__64_2'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_3'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldc.i4.5
      IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
      IL_0006:  ret
    } // end of method '<>c'::'<NewObj>b__64_3'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_4'() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::.ctor()
      IL_0005:  ret
    } // end of method '<>c'::'<NewObj>b__64_4'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_5'() cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>::.ctor()
      IL_0005:  ret
    } // end of method '<>c'::'<NewObj>b__64_5'

    .method assembly hidebysig instance object 
            '<NewObj>b__64_6'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldc.i4.5
      IL_0001:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
      IL_0006:  ret
    } // end of method '<>c'::'<NewObj>b__64_6'

    .method assembly hidebysig instance class [mscorlib]System.Type 
            '<TypeOfExpr>b__65_0'() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  ldtoken    [mscorlib]System.Int32
      IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000a:  ret
    } // end of method '<>c'::'<TypeOfExpr>b__65_0'

    .method assembly hidebysig instance class [mscorlib]System.Type 
            '<TypeOfExpr>b__65_1'() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  ldtoken    [mscorlib]System.Object
      IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000a:  ret
    } // end of method '<>c'::'<TypeOfExpr>b__65_1'

    .method assembly hidebysig instance class [mscorlib]System.Type 
            '<TypeOfExpr>b__65_2'() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  ldtoken    [mscorlib]System.Collections.Generic.List`1
      IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000a:  ret
    } // end of method '<>c'::'<TypeOfExpr>b__65_2'

    .method assembly hidebysig instance class [mscorlib]System.Type 
            '<TypeOfExpr>b__65_3'() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
      IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000a:  ret
    } // end of method '<>c'::'<TypeOfExpr>b__65_3'

    .method assembly hidebysig instance class [mscorlib]System.Type 
            '<TypeOfExpr>b__65_4'() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  ldtoken    int32*
      IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000a:  ret
    } // end of method '<>c'::'<TypeOfExpr>b__65_4'

    .method assembly hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass 
            '<AsTypeExpr>b__66_0'(object obj) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
      IL_0006:  ret
    } // end of method '<>c'::'<AsTypeExpr>b__66_0'

    .method assembly hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> 
            '<AsTypeExpr>b__66_1'(object obj) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
      IL_0006:  ret
    } // end of method '<>c'::'<AsTypeExpr>b__66_1'

    .method assembly hidebysig instance bool 
            '<IsTypeExpr>b__67_0'(object obj) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
      IL_0006:  ldnull
      IL_0007:  cgt.un
      IL_0009:  ret
    } // end of method '<>c'::'<IsTypeExpr>b__67_0'

    .method assembly hidebysig instance bool 
            '<UnaryLogicalOperators>b__68_0'(bool a) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.0
      IL_0002:  ceq
      IL_0004:  ret
    } // end of method '<>c'::'<UnaryLogicalOperators>b__68_0'

    .method assembly hidebysig instance int32 
            '<UnaryArithmeticOperators>b__71_0'(int32 a) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method '<>c'::'<UnaryArithmeticOperators>b__71_0'

    .method assembly hidebysig instance int32 
            '<UnaryArithmeticOperators>b__71_1'(int32 a) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  neg
      IL_0002:  ret
    } // end of method '<>c'::'<UnaryArithmeticOperators>b__71_1'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_0'(int32 a,
                                                 int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  add
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_0'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_1'(int32 a,
                                                 int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  sub
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_1'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_2'(int32 a,
                                                 int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  mul
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_2'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_3'(int32 a,
                                                 int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  div
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_3'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_4'(int32 a,
                                                 int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  rem
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_4'

    .method assembly hidebysig instance int64 
            '<BinaryArithmeticOperators>b__72_5'(int64 a,
                                                 int32 b) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  conv.i8
      IL_0003:  add
      IL_0004:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_5'

    .method assembly hidebysig instance int64 
            '<BinaryArithmeticOperators>b__72_6'(int64 a,
                                                 int32 b) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  conv.i8
      IL_0003:  sub
      IL_0004:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_6'

    .method assembly hidebysig instance int64 
            '<BinaryArithmeticOperators>b__72_7'(int64 a,
                                                 int32 b) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  conv.i8
      IL_0003:  mul
      IL_0004:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_7'

    .method assembly hidebysig instance int64 
            '<BinaryArithmeticOperators>b__72_8'(int64 a,
                                                 int32 b) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  conv.i8
      IL_0003:  div
      IL_0004:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_8'

    .method assembly hidebysig instance int64 
            '<BinaryArithmeticOperators>b__72_9'(int64 a,
                                                 int32 b) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  conv.i8
      IL_0003:  rem
      IL_0004:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_9'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_10'(int16 a,
                                                  int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  add
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_10'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_11'(int32 a,
                                                  int16 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  sub
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_11'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_12'(int16 a,
                                                  int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  mul
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_12'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_13'(int32 a,
                                                  int16 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  div
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_13'

    .method assembly hidebysig instance int32 
            '<BinaryArithmeticOperators>b__72_14'(int16 a,
                                                  int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  rem
      IL_0003:  ret
    } // end of method '<>c'::'<BinaryArithmeticOperators>b__72_14'

    .method assembly hidebysig instance int32 
            '<BitOperators>b__73_0'(int32 a) cil managed
    {
      // Code size       3 (0x3)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  not
      IL_0002:  ret
    } // end of method '<>c'::'<BitOperators>b__73_0'

    .method assembly hidebysig instance int32 
            '<BitOperators>b__73_1'(int32 a,
                                    int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  and
      IL_0003:  ret
    } // end of method '<>c'::'<BitOperators>b__73_1'

    .method assembly hidebysig instance int32 
            '<BitOperators>b__73_2'(int32 a,
                                    int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  or
      IL_0003:  ret
    } // end of method '<>c'::'<BitOperators>b__73_2'

    .method assembly hidebysig instance int32 
            '<BitOperators>b__73_3'(int32 a,
                                    int32 b) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  xor
      IL_0003:  ret
    } // end of method '<>c'::'<BitOperators>b__73_3'

    .method assembly hidebysig instance int32 
            '<ShiftOperators>b__74_0'(int32 a) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.2
      IL_0002:  shr
      IL_0003:  ret
    } // end of method '<>c'::'<ShiftOperators>b__74_0'

    .method assembly hidebysig instance int32 
            '<ShiftOperators>b__74_1'(int32 a) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.2
      IL_0002:  shl
      IL_0003:  ret
    } // end of method '<>c'::'<ShiftOperators>b__74_1'

    .method assembly hidebysig instance int64 
            '<ShiftOperators>b__74_2'(int64 a) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.2
      IL_0002:  shr
      IL_0003:  ret
    } // end of method '<>c'::'<ShiftOperators>b__74_2'

    .method assembly hidebysig instance int64 
            '<ShiftOperators>b__74_3'(int64 a) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.2
      IL_0002:  shl
      IL_0003:  ret
    } // end of method '<>c'::'<ShiftOperators>b__74_3'

    .method assembly hidebysig instance int32 
            '<SimpleExpressions>b__75_0'() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method '<>c'::'<SimpleExpressions>b__75_0'

    .method assembly hidebysig instance int32 
            '<SimpleExpressions>b__75_1'(int32 a) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } // end of method '<>c'::'<SimpleExpressions>b__75_1'

    .method assembly hidebysig instance string 
            '<Call>b__78_0'(string a) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0006:  ret
    } // end of method '<>c'::'<Call>b__78_0'

    .method assembly hidebysig instance string 
            '<Call>b__78_1'(int32 a) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarga.s   a
      IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
      IL_0007:  ret
    } // end of method '<>c'::'<Call>b__78_1'

    .method assembly hidebysig instance char[] 
            '<Call>b__78_2'(string a) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0006:  ret
    } // end of method '<>c'::'<Call>b__78_2'

    .method assembly hidebysig instance bool 
            '<Call>b__78_3'() cil managed
    {
      // Code size       16 (0x10)
      .maxstack  2
      .locals init (char V_0)
      IL_0000:  ldc.i4.s   97
      IL_0002:  stloc.0
      IL_0003:  ldloca.s   V_0
      IL_0005:  ldc.i4.s   98
      IL_0007:  call       instance int32 [mscorlib]System.Char::CompareTo(char)
      IL_000c:  ldc.i4.0
      IL_000d:  clt
      IL_000f:  ret
    } // end of method '<>c'::'<Call>b__78_3'

    .method assembly hidebysig instance bool 
            '<Quote>b__79_0'() cil managed
    {
      // Code size       113 (0x71)
      .maxstack  5
      .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
               class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
      IL_0000:  ldtoken    [mscorlib]System.Int32
      IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000a:  ldstr      "n"
      IL_000f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                     string)
      IL_0014:  stloc.0
      IL_0015:  ldtoken    [mscorlib]System.String
      IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_001f:  ldstr      "s"
      IL_0024:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                     string)
      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  ldloc.0
      IL_002c:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
      IL_0031:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
      IL_0036:  castclass  [mscorlib]System.Reflection.MethodInfo
      IL_003b:  ldc.i4.0
      IL_003c:  newarr     [System.Core]System.Linq.Expressions.Expression
      IL_0041:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
      IL_0046:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                         string)
      IL_004b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
      IL_0050:  castclass  [mscorlib]System.Reflection.MethodInfo
      IL_0055:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
      IL_005a:  ldc.i4.2
      IL_005b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
      IL_0060:  dup
      IL_0061:  ldc.i4.0
      IL_0062:  ldloc.0
      IL_0063:  stelem.ref
      IL_0064:  dup
      IL_0065:  ldc.i4.1
      IL_0066:  ldloc.1
      IL_0067:  stelem.ref
      IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
      IL_006d:  ldnull
      IL_006e:  cgt.un
      IL_0070:  ret
    } // end of method '<>c'::'<Quote>b__79_0'

    .method assembly hidebysig instance int32[] 
            '<ArrayInitializer>b__80_0'() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldc.i4.3
      IL_0001:  newarr     [mscorlib]System.Int32
      IL_0006:  dup
      IL_0007:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::E429CCA3F703A39CC5954A6572FEC9086135B34E
      IL_000c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                          valuetype [mscorlib]System.RuntimeFieldHandle)
      IL_0011:  ret
    } // end of method '<>c'::'<ArrayInitializer>b__80_0'

    .method assembly hidebysig instance int32[] 
            '<ArrayInitializer>b__80_1'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldc.i4.3
      IL_0001:  newarr     [mscorlib]System.Int32
      IL_0006:  ret
    } // end of method '<>c'::'<ArrayInitializer>b__80_1'

    .method assembly hidebysig instance int32[0...,0...] 
            '<ArrayInitializer>b__80_2'() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldc.i4.3
      IL_0001:  ldc.i4.5
      IL_0002:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                                 int32)
      IL_0007:  ret
    } // end of method '<>c'::'<ArrayInitializer>b__80_2'

    .method assembly hidebysig instance int32[][] 
            '<ArrayInitializer>b__80_3'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldc.i4.3
      IL_0001:  newarr     int32[]
      IL_0006:  ret
    } // end of method '<>c'::'<ArrayInitializer>b__80_3'

    .method assembly hidebysig instance int32[][] 
            '<ArrayInitializer>b__80_4'() cil managed
    {
      // Code size       27 (0x1b)
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  newarr     int32[]
      IL_0006:  dup
      IL_0007:  ldc.i4.0
      IL_0008:  ldc.i4.3
      IL_0009:  newarr     [mscorlib]System.Int32
      IL_000e:  dup
      IL_000f:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::E429CCA3F703A39CC5954A6572FEC9086135B34E
      IL_0014:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                          valuetype [mscorlib]System.RuntimeFieldHandle)
      IL_0019:  stelem.ref
      IL_001a:  ret
    } // end of method '<>c'::'<ArrayInitializer>b__80_4'

    .method assembly hidebysig instance object 
            '<AnonymousTypes>b__81_0'() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldc.i4.5
      IL_0001:  ldstr      "Test"
      IL_0006:  newobj     instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                            !1)
      IL_000b:  ret
    } // end of method '<>c'::'<AnonymousTypes>b__81_0'

  } // end of class '<>c'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass25_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string x
    .field public int32 i
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass25_0'::.ctor

  } // end of class '<>c__DisplayClass25_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass26_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public uint8 z
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass26_0'::.ctor

  } // end of class '<>c__DisplayClass26_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass33_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [System.Core]System.Collections.Generic.HashSet`1<int32> set
    .field public class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> sink
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass33_0'::.ctor

  } // end of class '<>c__DisplayClass33_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass36_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> 'call'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass36_0'::.ctor

  } // end of class '<>c__DisplayClass36_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass46_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public uint8 z
    .field public int32 y
    .field public bool x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass46_0'::.ctor

  } // end of class '<>c__DisplayClass46_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass47_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [System.Xml]System.Xml.XmlReaderSettings s
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass47_0'::.ctor

  } // end of class '<>c__DisplayClass47_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass57_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string x
    .field public int32 i
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass57_0'::.ctor

  } // end of class '<>c__DisplayClass57_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass76_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 captured
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass76_0'::.ctor

    .method assembly hidebysig instance int32 
            '<Capturing>b__0'() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::captured
      IL_0006:  ret
    } // end of method '<>c__DisplayClass76_0'::'<Capturing>b__0'

  } // end of class '<>c__DisplayClass76_0'

  .field private int32 'field'
  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Action`1<!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,R>(object x,
                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!T,!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,T2,R>(object x,
                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!T,!!T2,!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          X() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method ExpressionTrees::X

  .method public hidebysig instance void 
          Parameter(bool a) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'::a
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0012:  ldloc.0
    IL_0013:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0022:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'::a
    IL_0027:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0031:  ldc.i4.0
    IL_0032:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0037:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0041:  pop
    IL_0042:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'::a
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0012:  ldloc.0
    IL_0013:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0022:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'::a
    IL_0027:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0031:  ldc.i4.0
    IL_0032:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0037:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0041:  pop
    IL_0042:  ret
  } // end of method ExpressionTrees::LocalVariable

  .method public hidebysig instance void 
          LambdaParameter() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Boolean
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "a"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0021:  dup
    IL_0022:  ldc.i4.0
    IL_0023:  ldloc.0
    IL_0024:  stelem.ref
    IL_0025:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,bool>(object,
                                                                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_002f:  pop
    IL_0030:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       119 (0x77)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'::x
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0012:  ldc.i4.1
    IL_0013:  box        [mscorlib]System.Int32
    IL_0018:  ldtoken    [mscorlib]System.Int32
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0027:  ldloc.0
    IL_0028:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'::x
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldc.i4.2
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0075:  pop
    IL_0076:  ret
  } // end of method ExpressionTrees::AddOperator

  .method public hidebysig instance void 
          AnonymousClasses() cil managed
  {
    // Code size       153 (0x99)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class '<>f__AnonymousType0`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_000a:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.2
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  dup
    IL_0020:  ldc.i4.0
    IL_0021:  ldc.i4.3
    IL_0022:  box        [mscorlib]System.Int32
    IL_0027:  ldtoken    [mscorlib]System.Int32
    IL_002c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0036:  stelem.ref
    IL_0037:  dup
    IL_0038:  ldc.i4.1
    IL_0039:  ldstr      "a"
    IL_003e:  ldtoken    [mscorlib]System.String
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  stelem.ref
    IL_004e:  ldc.i4.2
    IL_004f:  newarr     [mscorlib]System.Reflection.MemberInfo
    IL_0054:  dup
    IL_0055:  ldc.i4.0
    IL_0056:  ldtoken    method instance !0 class '<>f__AnonymousType0`2'<int32,string>::get_X()
    IL_005b:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0060:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006a:  stelem.ref
    IL_006b:  dup
    IL_006c:  ldc.i4.1
    IL_006d:  ldtoken    method instance !1 class '<>f__AnonymousType0`2'<int32,string>::get_A()
    IL_0072:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0077:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0081:  stelem.ref
    IL_0082:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_0087:  ldc.i4.0
    IL_0088:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class '<>f__AnonymousType0`2'<int32,string>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0092:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0097:  pop
    IL_0098:  ret
  } // end of method ExpressionTrees::AnonymousClasses

  .method public hidebysig instance void 
          ArrayIndex() cil managed
  {
    // Code size       230 (0xe6)
    .maxstack  7
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.3
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  dup
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.3
    IL_0018:  box        [mscorlib]System.Int32
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002c:  stelem.ref
    IL_002d:  dup
    IL_002e:  ldc.i4.1
    IL_002f:  ldc.i4.4
    IL_0030:  box        [mscorlib]System.Int32
    IL_0035:  ldtoken    [mscorlib]System.Int32
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0044:  stelem.ref
    IL_0045:  dup
    IL_0046:  ldc.i4.2
    IL_0047:  ldc.i4.5
    IL_0048:  box        [mscorlib]System.Int32
    IL_004d:  ldtoken    [mscorlib]System.Int32
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005c:  stelem.ref
    IL_005d:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0062:  ldc.i4.0
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  ldnull
    IL_0078:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_007d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0082:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0087:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008c:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0091:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0096:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a0:  ldc.i4.3
    IL_00a1:  conv.i8
    IL_00a2:  box        [mscorlib]System.Int64
    IL_00a7:  ldtoken    [mscorlib]System.Int64
    IL_00ac:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b6:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_00bb:  ldtoken    [mscorlib]System.Int32
    IL_00c0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00cf:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00d4:  ldc.i4.0
    IL_00d5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00da:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00df:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e4:  pop
    IL_00e5:  ret
  } // end of method ExpressionTrees::ArrayIndex

  .method public hidebysig instance void 
          ArrayLengthAndDoubles() cil managed
  {
    // Code size       293 (0x125)
    .maxstack  17
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldnull
    IL_001e:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Concat<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0023:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0028:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002d:  ldc.i4.2
    IL_002e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0033:  dup
    IL_0034:  ldc.i4.0
    IL_0035:  ldtoken    [mscorlib]System.Double
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  ldc.i4.3
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0045:  dup
    IL_0046:  ldc.i4.0
    IL_0047:  ldc.r8     1.
    IL_0050:  box        [mscorlib]System.Double
    IL_0055:  ldtoken    [mscorlib]System.Double
    IL_005a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0064:  stelem.ref
    IL_0065:  dup
    IL_0066:  ldc.i4.1
    IL_0067:  ldc.r8     2.0099999999999998
    IL_0070:  box        [mscorlib]System.Double
    IL_0075:  ldtoken    [mscorlib]System.Double
    IL_007a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0084:  stelem.ref
    IL_0085:  dup
    IL_0086:  ldc.i4.2
    IL_0087:  ldc.r8     3.5
    IL_0090:  box        [mscorlib]System.Double
    IL_0095:  ldtoken    [mscorlib]System.Double
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a4:  stelem.ref
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00aa:  stelem.ref
    IL_00ab:  dup
    IL_00ac:  ldc.i4.1
    IL_00ad:  ldtoken    [mscorlib]System.Double
    IL_00b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b7:  ldc.i4.2
    IL_00b8:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bd:  dup
    IL_00be:  ldc.i4.0
    IL_00bf:  ldc.r8     1.
    IL_00c8:  box        [mscorlib]System.Double
    IL_00cd:  ldtoken    [mscorlib]System.Double
    IL_00d2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00dc:  stelem.ref
    IL_00dd:  dup
    IL_00de:  ldc.i4.1
    IL_00df:  ldc.r8     2.
    IL_00e8:  box        [mscorlib]System.Double
    IL_00ed:  ldtoken    [mscorlib]System.Double
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fc:  stelem.ref
    IL_00fd:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0102:  stelem.ref
    IL_0103:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0108:  stelem.ref
    IL_0109:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_010e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_0113:  ldc.i4.0
    IL_0114:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0119:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0123:  pop
    IL_0124:  ret
  } // end of method ExpressionTrees::ArrayLengthAndDoubles

  .method public hidebysig instance void 
          AsOperator() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Object
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldtoken    [mscorlib]System.String
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_0023:  ldc.i4.0
    IL_0024:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0029:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0033:  pop
    IL_0034:  ret
  } // end of method ExpressionTrees::AsOperator

  .method public hidebysig instance void 
          ComplexGenericName() cil managed
  {
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "x"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  ldtoken    [mscorlib]System.Int32
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0035:  ldc.i4.1
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  dup
    IL_003c:  ldc.i4.0
    IL_003d:  ldloc.0
    IL_003e:  stelem.ref
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0049:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0053:  ldc.i4.1
    IL_0054:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0059:  dup
    IL_005a:  ldc.i4.0
    IL_005b:  ldc.i4.0
    IL_005c:  box        [mscorlib]System.Int32
    IL_0061:  ldtoken    [mscorlib]System.Int32
    IL_0066:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0070:  stelem.ref
    IL_0071:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0076:  ldc.i4.0
    IL_0077:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0081:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0086:  pop
    IL_0087:  ret
  } // end of method ExpressionTrees::ComplexGenericName

  .method public hidebysig instance void 
          DefaultValue() cil managed
  {
    // Code size       171 (0xab)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.TimeSpan V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.TimeSpan::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.3
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  dup
    IL_001b:  ldc.i4.0
    IL_001c:  ldc.i4.1
    IL_001d:  box        [mscorlib]System.Int32
    IL_0022:  ldtoken    [mscorlib]System.Int32
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  stelem.ref
    IL_0032:  dup
    IL_0033:  ldc.i4.1
    IL_0034:  ldc.i4.2
    IL_0035:  box        [mscorlib]System.Int32
    IL_003a:  ldtoken    [mscorlib]System.Int32
    IL_003f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0044:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0049:  stelem.ref
    IL_004a:  dup
    IL_004b:  ldc.i4.2
    IL_004c:  ldc.i4.3
    IL_004d:  box        [mscorlib]System.Int32
    IL_0052:  ldtoken    [mscorlib]System.Int32
    IL_0057:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0061:  stelem.ref
    IL_0062:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0067:  ldloca.s   V_0
    IL_0069:  initobj    [mscorlib]System.TimeSpan
    IL_006f:  ldloc.0
    IL_0070:  box        [mscorlib]System.TimeSpan
    IL_0075:  ldtoken    [mscorlib]System.TimeSpan
    IL_007a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0084:  ldc.i4.0
    IL_0085:  ldtoken    method bool [mscorlib]System.TimeSpan::op_Equality(valuetype [mscorlib]System.TimeSpan,
                                                                            valuetype [mscorlib]System.TimeSpan)
    IL_008a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0094:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0099:  ldc.i4.0
    IL_009a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a4:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00a9:  pop
    IL_00aa:  ret
  } // end of method ExpressionTrees::DefaultValue

  .method public hidebysig instance void 
          EnumConstant() cil managed
  {
    // Code size       103 (0x67)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Object
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0019:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_001e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0023:  ldc.i4.1
    IL_0024:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0029:  dup
    IL_002a:  ldc.i4.0
    IL_002b:  ldc.i4.0
    IL_002c:  box        [mscorlib]System.MidpointRounding
    IL_0031:  ldtoken    [mscorlib]System.MidpointRounding
    IL_0036:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0040:  ldtoken    [mscorlib]System.Object
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0055:  ldc.i4.0
    IL_0056:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0060:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0065:  pop
    IL_0066:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       190 (0xbe)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  ldc.i4.s   20
    IL_000a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_000f:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__18_0'
    IL_0014:  dup
    IL_0015:  brtrue.s   IL_002e

    IL_0017:  pop
    IL_0018:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_001d:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<IndexerAccess>b__18_0'(int32)
    IL_0023:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0028:  dup
    IL_0029:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__18_0'
    IL_002e:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0033:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'::dict
    IL_0038:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_003d:  ldloc.0
    IL_003e:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'::dict
    IL_0052:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_005c:  ldtoken    method instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
    IL_0061:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>
    IL_0066:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0070:  ldc.i4.1
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0076:  dup
    IL_0077:  ldc.i4.0
    IL_0078:  ldstr      "3"
    IL_007d:  ldtoken    [mscorlib]System.String
    IL_0082:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0087:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008c:  stelem.ref
    IL_008d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0092:  ldc.i4.3
    IL_0093:  box        [mscorlib]System.Int32
    IL_0098:  ldtoken    [mscorlib]System.Int32
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ac:  ldc.i4.0
    IL_00ad:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bc:  pop
    IL_00bd:  ret
  } // end of method ExpressionTrees::IndexerAccess

  .method public hidebysig instance void 
          IsOperator() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Object
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldtoken    [mscorlib]System.String
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldc.i4.0
    IL_0024:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0029:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0033:  pop
    IL_0034:  ret
  } // end of method ExpressionTrees::IsOperator

  .method public hidebysig instance void 
          ListInitializer() cil managed
  {
    // Code size       346 (0x15a)
    .maxstack  11
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldc.i4.3
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_001a:  dup
    IL_001b:  ldc.i4.0
    IL_001c:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0021:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0026:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0030:  ldc.i4.2
    IL_0031:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0036:  dup
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4.1
    IL_0039:  box        [mscorlib]System.Int32
    IL_003e:  ldtoken    [mscorlib]System.Int32
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  stelem.ref
    IL_004e:  dup
    IL_004f:  ldc.i4.1
    IL_0050:  ldc.i4.1
    IL_0051:  box        [mscorlib]System.Int32
    IL_0056:  ldtoken    [mscorlib]System.Int32
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  stelem.ref
    IL_0066:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_006b:  stelem.ref
    IL_006c:  dup
    IL_006d:  ldc.i4.1
    IL_006e:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0073:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0078:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0082:  ldc.i4.2
    IL_0083:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0088:  dup
    IL_0089:  ldc.i4.0
    IL_008a:  ldc.i4.2
    IL_008b:  box        [mscorlib]System.Int32
    IL_0090:  ldtoken    [mscorlib]System.Int32
    IL_0095:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009f:  stelem.ref
    IL_00a0:  dup
    IL_00a1:  ldc.i4.1
    IL_00a2:  ldc.i4.2
    IL_00a3:  box        [mscorlib]System.Int32
    IL_00a8:  ldtoken    [mscorlib]System.Int32
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  stelem.ref
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00bd:  stelem.ref
    IL_00be:  dup
    IL_00bf:  ldc.i4.2
    IL_00c0:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_00c5:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_00ca:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d4:  ldc.i4.2
    IL_00d5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00da:  dup
    IL_00db:  ldc.i4.0
    IL_00dc:  ldc.i4.3
    IL_00dd:  box        [mscorlib]System.Int32
    IL_00e2:  ldtoken    [mscorlib]System.Int32
    IL_00e7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f1:  stelem.ref
    IL_00f2:  dup
    IL_00f3:  ldc.i4.1
    IL_00f4:  ldc.i4.4
    IL_00f5:  box        [mscorlib]System.Int32
    IL_00fa:  ldtoken    [mscorlib]System.Int32
    IL_00ff:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0104:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0109:  stelem.ref
    IL_010a:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_010f:  stelem.ref
    IL_0110:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_0115:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::get_Count()
    IL_011a:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_011f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0124:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0129:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012e:  ldc.i4.3
    IL_012f:  box        [mscorlib]System.Int32
    IL_0134:  ldtoken    [mscorlib]System.Int32
    IL_0139:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0143:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0148:  ldc.i4.0
    IL_0149:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0153:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0158:  pop
    IL_0159:  ret
  } // end of method ExpressionTrees::ListInitializer

  .method public hidebysig instance void 
          ListInitializer2() cil managed
  {
    // Code size       315 (0x13b)
    .maxstack  11
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.1
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  dup
    IL_0020:  ldc.i4.0
    IL_0021:  ldc.i4.s   50
    IL_0023:  box        [mscorlib]System.Int32
    IL_0028:  ldtoken    [mscorlib]System.Int32
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  stelem.ref
    IL_0038:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_003d:  ldc.i4.3
    IL_003e:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_0043:  dup
    IL_0044:  ldc.i4.0
    IL_0045:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_004a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_004f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0054:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0059:  ldc.i4.1
    IL_005a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005f:  dup
    IL_0060:  ldc.i4.0
    IL_0061:  ldc.i4.1
    IL_0062:  box        [mscorlib]System.Int32
    IL_0067:  ldtoken    [mscorlib]System.Int32
    IL_006c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0071:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0076:  stelem.ref
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_007c:  stelem.ref
    IL_007d:  dup
    IL_007e:  ldc.i4.1
    IL_007f:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0084:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0089:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0093:  ldc.i4.1
    IL_0094:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0099:  dup
    IL_009a:  ldc.i4.0
    IL_009b:  ldc.i4.2
    IL_009c:  box        [mscorlib]System.Int32
    IL_00a1:  ldtoken    [mscorlib]System.Int32
    IL_00a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b0:  stelem.ref
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b6:  stelem.ref
    IL_00b7:  dup
    IL_00b8:  ldc.i4.2
    IL_00b9:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00be:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00c3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00cd:  ldc.i4.1
    IL_00ce:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00d3:  dup
    IL_00d4:  ldc.i4.0
    IL_00d5:  ldc.i4.3
    IL_00d6:  box        [mscorlib]System.Int32
    IL_00db:  ldtoken    [mscorlib]System.Int32
    IL_00e0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ea:  stelem.ref
    IL_00eb:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f0:  stelem.ref
    IL_00f1:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00f6:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00fb:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0100:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_010a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010f:  ldc.i4.3
    IL_0110:  box        [mscorlib]System.Int32
    IL_0115:  ldtoken    [mscorlib]System.Int32
    IL_011a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0124:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0129:  ldc.i4.0
    IL_012a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0134:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0139:  pop
    IL_013a:  ret
  } // end of method ExpressionTrees::ListInitializer2

  .method public hidebysig instance void 
          ListInitializer3() cil managed
  {
    // Code size       274 (0x112)
    .maxstack  11
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldc.i4.3
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_001a:  dup
    IL_001b:  ldc.i4.0
    IL_001c:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0021:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0026:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0030:  ldc.i4.1
    IL_0031:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0036:  dup
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4.1
    IL_0039:  box        [mscorlib]System.Int32
    IL_003e:  ldtoken    [mscorlib]System.Int32
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  stelem.ref
    IL_004e:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0053:  stelem.ref
    IL_0054:  dup
    IL_0055:  ldc.i4.1
    IL_0056:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_005b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0060:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006a:  ldc.i4.1
    IL_006b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0070:  dup
    IL_0071:  ldc.i4.0
    IL_0072:  ldc.i4.2
    IL_0073:  box        [mscorlib]System.Int32
    IL_0078:  ldtoken    [mscorlib]System.Int32
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0087:  stelem.ref
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_008d:  stelem.ref
    IL_008e:  dup
    IL_008f:  ldc.i4.2
    IL_0090:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0095:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_009a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a4:  ldc.i4.1
    IL_00a5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00aa:  dup
    IL_00ab:  ldc.i4.0
    IL_00ac:  ldc.i4.3
    IL_00ad:  box        [mscorlib]System.Int32
    IL_00b2:  ldtoken    [mscorlib]System.Int32
    IL_00b7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c1:  stelem.ref
    IL_00c2:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c7:  stelem.ref
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00cd:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00d2:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00d7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00e6:  ldc.i4.3
    IL_00e7:  box        [mscorlib]System.Int32
    IL_00ec:  ldtoken    [mscorlib]System.Int32
    IL_00f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0100:  ldc.i4.0
    IL_0101:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0106:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0110:  pop
    IL_0111:  ret
  } // end of method ExpressionTrees::ListInitializer3

  .method public hidebysig instance void 
          LiteralCharAndProperty() cil managed
  {
    // Code size       144 (0x90)
    .maxstack  7
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.String::.ctor(char,
                                                                             int32)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.2
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  dup
    IL_001b:  ldc.i4.0
    IL_001c:  ldc.i4.s   32
    IL_001e:  box        [mscorlib]System.Char
    IL_0023:  ldtoken    [mscorlib]System.Char
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  stelem.ref
    IL_0033:  dup
    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.3
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  ldtoken    [mscorlib]System.Int32
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004a:  stelem.ref
    IL_004b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0050:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0055:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_005a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0064:  ldc.i4.1
    IL_0065:  box        [mscorlib]System.Int32
    IL_006a:  ldtoken    [mscorlib]System.Int32
    IL_006f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0074:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0079:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_007e:  ldc.i4.0
    IL_007f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0084:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0089:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008e:  pop
    IL_008f:  ret
  } // end of method ExpressionTrees::LiteralCharAndProperty

  .method public hidebysig instance void 
          CharNoCast() cil managed
  {
    // Code size       135 (0x87)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldstr      "abc"
    IL_000a:  ldtoken    [mscorlib]System.String
    IL_000f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0019:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0023:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0028:  ldc.i4.1
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  dup
    IL_002f:  ldc.i4.0
    IL_0030:  ldc.i4.1
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005a:  ldc.i4.s   98
    IL_005c:  box        [mscorlib]System.Int32
    IL_0061:  ldtoken    [mscorlib]System.Int32
    IL_0066:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0075:  ldc.i4.0
    IL_0076:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0080:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0085:  pop
    IL_0086:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       406 (0x196)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::i
    IL_000d:  ldloc.0
    IL_000e:  ldstr      "X"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001d:  ldstr      "a\n\\b"
    IL_0022:  ldtoken    [mscorlib]System.String
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0041:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_0046:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0050:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0055:  ldloc.0
    IL_0056:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_006a:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0074:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0088:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_008d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0092:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0097:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_009c:  ldc.i4.2
    IL_009d:  box        [mscorlib]System.Int32
    IL_00a2:  ldtoken    [mscorlib]System.Int32
    IL_00a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00b6:  ldc.i4.0
    IL_00b7:  box        [mscorlib]System.Boolean
    IL_00bc:  ldtoken    [mscorlib]System.Boolean
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cb:  ldc.i4.1
    IL_00cc:  box        [mscorlib]System.Boolean
    IL_00d1:  ldtoken    [mscorlib]System.Boolean
    IL_00d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00db:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e0:  ldc.i4.1
    IL_00e1:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00e6:  box        [mscorlib]System.Decimal
    IL_00eb:  ldtoken    [mscorlib]System.Decimal
    IL_00f0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fa:  ldloc.0
    IL_00fb:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'
    IL_0100:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010a:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::i
    IL_010f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0114:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0119:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_011e:  ldtoken    [mscorlib]System.Decimal
    IL_0123:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0128:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_012d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0132:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0137:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_013c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0141:  ldc.i4.0
    IL_0142:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0147:  box        [mscorlib]System.Decimal
    IL_014c:  ldtoken    [mscorlib]System.Decimal
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0160:  ldc.i4.0
    IL_0161:  box        [mscorlib]System.Boolean
    IL_0166:  ldtoken    [mscorlib]System.Boolean
    IL_016b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0170:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0175:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_017a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_017f:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0184:  ldc.i4.0
    IL_0185:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0194:  pop
    IL_0195:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       114 (0x72)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s   42
    IL_0009:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'::z
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'::z
    IL_0028:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0032:  ldtoken    [mscorlib]System.Int32
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0046:  ldc.i4.0
    IL_0047:  box        [mscorlib]System.Int32
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0060:  ldc.i4.0
    IL_0061:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0066:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0070:  pop
    IL_0071:  ret
  } // end of method ExpressionTrees::NotImplicitCast

  .method public hidebysig instance void 
          MembersBuiltin() cil managed
  {
    // Code size       401 (0x191)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldc.i4.s   123
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.2
    IL_000b:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0010:  box        [mscorlib]System.Decimal
    IL_0015:  ldtoken    [mscorlib]System.Decimal
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldtoken    method instance string [mscorlib]System.Decimal::ToString()
    IL_0029:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0033:  ldc.i4.0
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0039:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004e:  pop
    IL_004f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0054:  ldc.i4     0x7fff
    IL_0059:  box        [mscorlib]System.AttributeTargets
    IL_005e:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  ldtoken    method instance bool [mscorlib]System.Enum::HasFlag(class [mscorlib]System.Enum)
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  ldc.i4.1
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0082:  dup
    IL_0083:  ldc.i4.0
    IL_0084:  ldc.i4.1
    IL_0085:  box        [mscorlib]System.AttributeTargets
    IL_008a:  ldtoken    [mscorlib]System.AttributeTargets
    IL_008f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0094:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0099:  ldtoken    [mscorlib]System.Enum
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a8:  stelem.ref
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ae:  ldc.i4.0
    IL_00af:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00be:  pop
    IL_00bf:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c4:  ldstr      "abc"
    IL_00c9:  ldtoken    [mscorlib]System.String
    IL_00ce:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d8:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_00dd:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e2:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ec:  ldc.i4.3
    IL_00ed:  box        [mscorlib]System.Int32
    IL_00f2:  ldtoken    [mscorlib]System.Int32
    IL_00f7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0101:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0106:  ldc.i4.0
    IL_0107:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0111:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0116:  pop
    IL_0117:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_011c:  ldc.i4.s   97
    IL_011e:  box        [mscorlib]System.Char
    IL_0123:  ldtoken    [mscorlib]System.Char
    IL_0128:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0132:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_0137:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_013c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0141:  ldc.i4.1
    IL_0142:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0147:  dup
    IL_0148:  ldc.i4.0
    IL_0149:  ldc.i4.s   98
    IL_014b:  box        [mscorlib]System.Char
    IL_0150:  ldtoken    [mscorlib]System.Char
    IL_0155:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015f:  stelem.ref
    IL_0160:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0165:  ldc.i4.0
    IL_0166:  box        [mscorlib]System.Int32
    IL_016b:  ldtoken    [mscorlib]System.Int32
    IL_0170:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0175:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_017f:  ldc.i4.0
    IL_0180:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0185:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_018f:  pop
    IL_0190:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       540 (0x21c)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.DateTime V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldloca.s   V_0
    IL_0007:  initobj    [mscorlib]System.DateTime
    IL_000d:  ldloc.0
    IL_000e:  box        [mscorlib]System.DateTime
    IL_0013:  ldtoken    [mscorlib]System.DateTime
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0022:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0027:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0031:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0036:  ldc.i4.0
    IL_0037:  conv.i8
    IL_0038:  box        [mscorlib]System.Int64
    IL_003d:  ldtoken    [mscorlib]System.Int64
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.0
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0061:  pop
    IL_0062:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0067:  ldnull
    IL_0068:  ldtoken    [mscorlib]System.Array
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_007c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0081:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0086:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008b:  ldc.i4.0
    IL_008c:  box        [mscorlib]System.Int32
    IL_0091:  ldtoken    [mscorlib]System.Int32
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a5:  ldc.i4.0
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b5:  pop
    IL_00b6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00bb:  ldnull
    IL_00bc:  ldtoken    [mscorlib]System.Type
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cb:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_00d0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00d5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00da:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00df:  ldc.i4.0
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ea:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ef:  pop
    IL_00f0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00f5:  ldnull
    IL_00f6:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0100:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0105:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_010a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0114:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0119:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011e:  ldc.i4.0
    IL_011f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0124:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0129:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012e:  pop
    IL_012f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0134:  ldnull
    IL_0135:  ldtoken    [mscorlib]System.Array
    IL_013a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0144:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_0149:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_014e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0153:  ldc.i4.0
    IL_0154:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0159:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_015e:  ldnull
    IL_015f:  ldtoken    [mscorlib]System.Object
    IL_0164:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0169:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_016e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0173:  ldc.i4.0
    IL_0174:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0179:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_017e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0183:  pop
    IL_0184:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0189:  ldnull
    IL_018a:  ldtoken    [mscorlib]System.Type
    IL_018f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0194:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0199:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_019e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01a3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01a8:  ldc.i4.1
    IL_01a9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01ae:  dup
    IL_01af:  ldc.i4.0
    IL_01b0:  ldtoken    [mscorlib]System.Object
    IL_01b5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ba:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_01bf:  stelem.ref
    IL_01c0:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01c5:  ldc.i4.0
    IL_01c6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01cb:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01d0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01d5:  pop
    IL_01d6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_01db:  ldnull
    IL_01dc:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_01e1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01eb:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_01f0:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_01f5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01fa:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01ff:  ldc.i4.0
    IL_0200:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0205:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_020a:  ldc.i4.0
    IL_020b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0210:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0215:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_021a:  pop
    IL_021b:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       346 (0x15a)
    .maxstack  9
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0015:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'field'
    IL_001a:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0024:  ldarg.0
    IL_0025:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::C()
    IL_0039:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0043:  ldc.i4.0
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0053:  ldc.i4.0
    IL_0054:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0063:  pop
    IL_0064:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0069:  ldnull
    IL_006a:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_006f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0074:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0079:  ldc.i4.2
    IL_007a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007f:  dup
    IL_0080:  ldc.i4.0
    IL_0081:  ldarg.0
    IL_0082:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0087:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0091:  stelem.ref
    IL_0092:  dup
    IL_0093:  ldc.i4.1
    IL_0094:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0099:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_00a3:  stelem.ref
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00ae:  ldc.i4.0
    IL_00af:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00be:  pop
    IL_00bf:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c4:  ldarg.0
    IL_00c5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00ca:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d4:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_00d9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00de:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e3:  ldc.i4.1
    IL_00e4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e9:  dup
    IL_00ea:  ldc.i4.0
    IL_00eb:  ldarg.0
    IL_00ec:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fb:  stelem.ref
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0101:  ldarg.0
    IL_0102:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0107:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0111:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_0116:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_011b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0120:  ldc.i4.1
    IL_0121:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0126:  dup
    IL_0127:  ldc.i4.0
    IL_0128:  ldnull
    IL_0129:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_012e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0133:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0138:  stelem.ref
    IL_0139:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_013e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0143:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0148:  ldc.i4.0
    IL_0149:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0153:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0158:  pop
    IL_0159:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees other) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       273 (0x111)
    .maxstack  12
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    method bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0014:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0028:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0032:  ldc.i4.2
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0038:  dup
    IL_0039:  ldc.i4.0
    IL_003a:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_003f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0044:  ldtoken    [mscorlib]System.Type
    IL_0049:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0053:  stelem.ref
    IL_0054:  dup
    IL_0055:  ldc.i4.1
    IL_0056:  ldtoken    [mscorlib]System.Int32
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  ldc.i4.4
    IL_0061:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0066:  dup
    IL_0067:  ldc.i4.0
    IL_0068:  ldc.i4     0x7d0
    IL_006d:  box        [mscorlib]System.Int32
    IL_0072:  ldtoken    [mscorlib]System.Int32
    IL_0077:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0081:  stelem.ref
    IL_0082:  dup
    IL_0083:  ldc.i4.1
    IL_0084:  ldc.i4     0x7d4
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  stelem.ref
    IL_009e:  dup
    IL_009f:  ldc.i4.2
    IL_00a0:  ldc.i4     0x7d8
    IL_00a5:  box        [mscorlib]System.Int32
    IL_00aa:  ldtoken    [mscorlib]System.Int32
    IL_00af:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b9:  stelem.ref
    IL_00ba:  dup
    IL_00bb:  ldc.i4.3
    IL_00bc:  ldc.i4     0x7dc
    IL_00c1:  box        [mscorlib]System.Int32
    IL_00c6:  ldtoken    [mscorlib]System.Int32
    IL_00cb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d5:  stelem.ref
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00db:  ldtoken    class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_00e0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00ea:  stelem.ref
    IL_00eb:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f0:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_00f5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00ff:  ldc.i4.0
    IL_0100:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0105:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_010f:  pop
    IL_0110:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       872 (0x368)
    .maxstack  13
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_000b:  ldnull
    IL_000c:  ldtoken    method bool [mscorlib]System.Array::TrueForAll<int32>(!!0[],
                                                                               class [mscorlib]System.Predicate`1<!!0>)
    IL_0011:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0016:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001b:  ldc.i4.2
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0021:  dup
    IL_0022:  ldc.i4.0
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  ldc.i4.4
    IL_002e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0033:  dup
    IL_0034:  ldc.i4.0
    IL_0035:  ldc.i4     0x7d0
    IL_003a:  box        [mscorlib]System.Int32
    IL_003f:  ldtoken    [mscorlib]System.Int32
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  dup
    IL_0050:  ldc.i4.1
    IL_0051:  ldc.i4     0x7d4
    IL_0056:  box        [mscorlib]System.Int32
    IL_005b:  ldtoken    [mscorlib]System.Int32
    IL_0060:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006a:  stelem.ref
    IL_006b:  dup
    IL_006c:  ldc.i4.2
    IL_006d:  ldc.i4     0x7d8
    IL_0072:  box        [mscorlib]System.Int32
    IL_0077:  ldtoken    [mscorlib]System.Int32
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0086:  stelem.ref
    IL_0087:  dup
    IL_0088:  ldc.i4.3
    IL_0089:  ldc.i4     0x7dc
    IL_008e:  box        [mscorlib]System.Int32
    IL_0093:  ldtoken    [mscorlib]System.Int32
    IL_0098:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a2:  stelem.ref
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a8:  stelem.ref
    IL_00a9:  dup
    IL_00aa:  ldc.i4.1
    IL_00ab:  ldtoken    method bool [mscorlib]System.DateTime::IsLeapYear(int32)
    IL_00b0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ba:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_00bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c9:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_00ce:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00d3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d8:  ldc.i4.2
    IL_00d9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00de:  dup
    IL_00df:  ldc.i4.0
    IL_00e0:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_00e5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ea:  ldtoken    [mscorlib]System.Type
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f9:  stelem.ref
    IL_00fa:  dup
    IL_00fb:  ldc.i4.1
    IL_00fc:  ldnull
    IL_00fd:  ldtoken    [mscorlib]System.Object
    IL_0102:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0107:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010c:  stelem.ref
    IL_010d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0112:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_0117:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0121:  stelem.ref
    IL_0122:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0127:  ldc.i4.0
    IL_0128:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0132:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0137:  pop
    IL_0138:  ldloc.0
    IL_0139:  newobj     instance void class [System.Core]System.Collections.Generic.HashSet`1<int32>::.ctor()
    IL_013e:  stfld      class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::set
    IL_0143:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0148:  ldnull
    IL_0149:  ldtoken    method bool [System.Core]System.Linq.Enumerable::All<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
    IL_014e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0153:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0158:  ldc.i4.2
    IL_0159:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015e:  dup
    IL_015f:  ldc.i4.0
    IL_0160:  ldtoken    [mscorlib]System.Int32
    IL_0165:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016a:  ldc.i4.4
    IL_016b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0170:  dup
    IL_0171:  ldc.i4.0
    IL_0172:  ldc.i4     0x7d0
    IL_0177:  box        [mscorlib]System.Int32
    IL_017c:  ldtoken    [mscorlib]System.Int32
    IL_0181:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0186:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_018b:  stelem.ref
    IL_018c:  dup
    IL_018d:  ldc.i4.1
    IL_018e:  ldc.i4     0x7d4
    IL_0193:  box        [mscorlib]System.Int32
    IL_0198:  ldtoken    [mscorlib]System.Int32
    IL_019d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a7:  stelem.ref
    IL_01a8:  dup
    IL_01a9:  ldc.i4.2
    IL_01aa:  ldc.i4     0x7d8
    IL_01af:  box        [mscorlib]System.Int32
    IL_01b4:  ldtoken    [mscorlib]System.Int32
    IL_01b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01be:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01c3:  stelem.ref
    IL_01c4:  dup
    IL_01c5:  ldc.i4.3
    IL_01c6:  ldc.i4     0x7dc
    IL_01cb:  box        [mscorlib]System.Int32
    IL_01d0:  ldtoken    [mscorlib]System.Int32
    IL_01d5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01da:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01df:  stelem.ref
    IL_01e0:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01e5:  stelem.ref
    IL_01e6:  dup
    IL_01e7:  ldc.i4.1
    IL_01e8:  ldtoken    method instance bool class [System.Core]System.Collections.Generic.HashSet`1<int32>::Add(!0)
    IL_01ed:  ldtoken    class [System.Core]System.Collections.Generic.HashSet`1<int32>
    IL_01f2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01fc:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0201:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0206:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020b:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0210:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0215:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_021a:  ldc.i4.2
    IL_021b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0220:  dup
    IL_0221:  ldc.i4.0
    IL_0222:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0227:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022c:  ldtoken    [mscorlib]System.Type
    IL_0231:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0236:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_023b:  stelem.ref
    IL_023c:  dup
    IL_023d:  ldc.i4.1
    IL_023e:  ldloc.0
    IL_023f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'
    IL_0244:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0249:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_024e:  ldtoken    field class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::set
    IL_0253:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0258:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_025d:  stelem.ref
    IL_025e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0263:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0268:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_026d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0272:  stelem.ref
    IL_0273:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0278:  ldc.i4.0
    IL_0279:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_027e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0283:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0288:  pop
    IL_0289:  ldloc.0
    IL_028a:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__33_0'
    IL_028f:  dup
    IL_0290:  brtrue.s   IL_02a9

    IL_0292:  pop
    IL_0293:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0298:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<MethodGroupConstant>b__33_0'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_029e:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_02a3:  dup
    IL_02a4:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__33_0'
    IL_02a9:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::sink
    IL_02ae:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_02b3:  ldloc.0
    IL_02b4:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'
    IL_02b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02be:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02c3:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::sink
    IL_02c8:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02cd:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02d2:  ldc.i4.1
    IL_02d3:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02d8:  dup
    IL_02d9:  ldc.i4.0
    IL_02da:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02df:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02e4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02e9:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_02ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02f8:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_02fd:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0302:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0307:  ldc.i4.2
    IL_0308:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_030d:  dup
    IL_030e:  ldc.i4.0
    IL_030f:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0314:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0319:  ldtoken    [mscorlib]System.Type
    IL_031e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0323:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0328:  stelem.ref
    IL_0329:  dup
    IL_032a:  ldc.i4.1
    IL_032b:  ldnull
    IL_032c:  ldtoken    [mscorlib]System.Object
    IL_0331:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0336:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_033b:  stelem.ref
    IL_033c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0341:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0346:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0350:  stelem.ref
    IL_0351:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0356:  ldc.i4.0
    IL_0357:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_035c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0361:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0366:  pop
    IL_0367:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       100 (0x64)
    .maxstack  4
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldc.i4.1
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  ldtoken    [mscorlib]System.Int32
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldc.i4.1
    IL_001b:  box        [mscorlib]System.Int32
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002f:  ldtoken    [mscorlib]System.Object
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_003e:  ldtoken    [mscorlib]System.Int32
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0062:  pop
    IL_0063:  ret
  } // end of method ExpressionTrees::MultipleCasts

  .method public hidebysig instance void 
          MultipleDots() cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  4
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldc.i4.3
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  ldtoken    [mscorlib]System.Int32
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.0
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0034:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0039:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0043:  ldc.i4.0
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004e:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0053:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0058:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0062:  ldc.i4.0
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_007c:  ldc.i4.0
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0082:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0087:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008c:  pop
    IL_008d:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       547 (0x223)
    .maxstack  12
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0' V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__36_0'
    IL_000c:  dup
    IL_000d:  brtrue.s   IL_0026

    IL_000f:  pop
    IL_0010:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0015:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NestedLambda>b__36_0'(class [mscorlib]System.Func`1<int32>)
    IL_001b:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_0020:  dup
    IL_0021:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__36_0'
    IL_0026:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'::'call'
    IL_002b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0030:  ldloc.0
    IL_0031:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'
    IL_0036:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0040:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'::'call'
    IL_0045:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_004f:  ldc.i4.1
    IL_0050:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0055:  dup
    IL_0056:  ldc.i4.0
    IL_0057:  ldc.i4.s   42
    IL_0059:  box        [mscorlib]System.Int32
    IL_005e:  ldtoken    [mscorlib]System.Int32
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  ldc.i4.0
    IL_006e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0073:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0078:  stelem.ref
    IL_0079:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_007e:  ldc.i4.0
    IL_007f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0084:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0089:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008e:  pop
    IL_008f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0094:  ldnull
    IL_0095:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_009a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a4:  ldc.i4.2
    IL_00a5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00aa:  dup
    IL_00ab:  ldc.i4.0
    IL_00ac:  ldtoken    [mscorlib]System.Int32
    IL_00b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b6:  ldc.i4.2
    IL_00b7:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bc:  dup
    IL_00bd:  ldc.i4.0
    IL_00be:  ldc.i4.s   37
    IL_00c0:  box        [mscorlib]System.Int32
    IL_00c5:  ldtoken    [mscorlib]System.Int32
    IL_00ca:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d4:  stelem.ref
    IL_00d5:  dup
    IL_00d6:  ldc.i4.1
    IL_00d7:  ldc.i4.s   42
    IL_00d9:  box        [mscorlib]System.Int32
    IL_00de:  ldtoken    [mscorlib]System.Int32
    IL_00e3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ed:  stelem.ref
    IL_00ee:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f3:  stelem.ref
    IL_00f4:  dup
    IL_00f5:  ldc.i4.1
    IL_00f6:  ldtoken    [mscorlib]System.Int32
    IL_00fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0100:  ldstr      "x"
    IL_0105:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010a:  stloc.1
    IL_010b:  ldloc.1
    IL_010c:  ldc.i4.2
    IL_010d:  box        [mscorlib]System.Int32
    IL_0112:  ldtoken    [mscorlib]System.Int32
    IL_0117:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0121:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0126:  ldc.i4.1
    IL_0127:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012c:  dup
    IL_012d:  ldc.i4.0
    IL_012e:  ldloc.1
    IL_012f:  stelem.ref
    IL_0130:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0135:  stelem.ref
    IL_0136:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_013b:  ldc.i4.0
    IL_013c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0141:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0146:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_014b:  pop
    IL_014c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0151:  ldnull
    IL_0152:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`3<!!0,int32,!!1>)
    IL_0157:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0161:  ldc.i4.2
    IL_0162:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0167:  dup
    IL_0168:  ldc.i4.0
    IL_0169:  ldtoken    [mscorlib]System.Int32
    IL_016e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0173:  ldc.i4.2
    IL_0174:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0179:  dup
    IL_017a:  ldc.i4.0
    IL_017b:  ldc.i4.s   37
    IL_017d:  box        [mscorlib]System.Int32
    IL_0182:  ldtoken    [mscorlib]System.Int32
    IL_0187:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0191:  stelem.ref
    IL_0192:  dup
    IL_0193:  ldc.i4.1
    IL_0194:  ldc.i4.s   42
    IL_0196:  box        [mscorlib]System.Int32
    IL_019b:  ldtoken    [mscorlib]System.Int32
    IL_01a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01aa:  stelem.ref
    IL_01ab:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01b0:  stelem.ref
    IL_01b1:  dup
    IL_01b2:  ldc.i4.1
    IL_01b3:  ldtoken    [mscorlib]System.Int32
    IL_01b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01bd:  ldstr      "x"
    IL_01c2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01c7:  stloc.1
    IL_01c8:  ldtoken    [mscorlib]System.Int32
    IL_01cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d2:  ldstr      "i"
    IL_01d7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01dc:  stloc.2
    IL_01dd:  ldloc.1
    IL_01de:  ldc.i4.2
    IL_01df:  box        [mscorlib]System.Int32
    IL_01e4:  ldtoken    [mscorlib]System.Int32
    IL_01e9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ee:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_01f8:  ldc.i4.2
    IL_01f9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01fe:  dup
    IL_01ff:  ldc.i4.0
    IL_0200:  ldloc.1
    IL_0201:  stelem.ref
    IL_0202:  dup
    IL_0203:  ldc.i4.1
    IL_0204:  ldloc.2
    IL_0205:  stelem.ref
    IL_0206:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_020b:  stelem.ref
    IL_020c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0211:  ldc.i4.0
    IL_0212:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0217:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_021c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0221:  pop
    IL_0222:  ret
  } // end of method ExpressionTrees::NestedLambda

  .method public hidebysig instance void 
          CurriedLambda() cil managed
  {
    // Code size       133 (0x85)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "a"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldtoken    [mscorlib]System.Int32
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  ldstr      "b"
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002e:  stloc.1
    IL_002f:  ldtoken    [mscorlib]System.Int32
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  ldstr      "c"
    IL_003e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0043:  stloc.2
    IL_0044:  ldloc.0
    IL_0045:  ldloc.1
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.2
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.1
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  dup
    IL_0058:  ldc.i4.0
    IL_0059:  ldloc.2
    IL_005a:  stelem.ref
    IL_005b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0060:  ldc.i4.1
    IL_0061:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0066:  dup
    IL_0067:  ldc.i4.0
    IL_0068:  ldloc.1
    IL_0069:  stelem.ref
    IL_006a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006f:  ldc.i4.1
    IL_0070:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0075:  dup
    IL_0076:  ldc.i4.0
    IL_0077:  ldloc.0
    IL_0078:  stelem.ref
    IL_0079:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0083:  pop
    IL_0084:  ret
  } // end of method ExpressionTrees::CurriedLambda

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   42
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0008:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Buzz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   42
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0008:  ret
  } // end of method ExpressionTrees::Buzz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<string,bool> a) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "42"
    IL_0006:  callvirt   instance !1 class [mscorlib]System.Func`2<string,bool>::Invoke(!0)
    IL_000b:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool> a) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>::Invoke(!0)
    IL_0007:  ret
  } // end of method ExpressionTrees::Fizz

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       1124 (0x464)
    .maxstack  14
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0015:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_001a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_001f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0024:  ldc.i4.1
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002a:  dup
    IL_002b:  ldc.i4.0
    IL_002c:  ldtoken    [mscorlib]System.String
    IL_0031:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0036:  ldstr      "x"
    IL_003b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0040:  stloc.0
    IL_0041:  ldloc.0
    IL_0042:  ldstr      "a"
    IL_0047:  ldtoken    [mscorlib]System.String
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_005b:  ldc.i4.1
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  dup
    IL_0062:  ldc.i4.0
    IL_0063:  ldloc.0
    IL_0064:  stelem.ref
    IL_0065:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006a:  stelem.ref
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0070:  ldc.i4.0
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0076:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0080:  pop
    IL_0081:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0086:  ldarg.0
    IL_0087:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_008c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0096:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_009b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00a0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a5:  ldc.i4.1
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ab:  dup
    IL_00ac:  ldc.i4.0
    IL_00ad:  ldtoken    [mscorlib]System.String
    IL_00b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b7:  ldstr      "x"
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00c1:  stloc.0
    IL_00c2:  ldloc.0
    IL_00c3:  ldstr      "a"
    IL_00c8:  ldtoken    [mscorlib]System.String
    IL_00cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00dc:  ldc.i4.1
    IL_00dd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e2:  dup
    IL_00e3:  ldc.i4.0
    IL_00e4:  ldloc.0
    IL_00e5:  stelem.ref
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00eb:  stelem.ref
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f1:  ldc.i4.0
    IL_00f2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00fc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0101:  pop
    IL_0102:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0107:  ldarg.0
    IL_0108:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_010d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0112:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0117:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_011c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0121:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0126:  ldc.i4.1
    IL_0127:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_012c:  dup
    IL_012d:  ldc.i4.0
    IL_012e:  ldtoken    [mscorlib]System.Action
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  ldstr      "x"
    IL_013d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0142:  stloc.0
    IL_0143:  ldloc.0
    IL_0144:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_0149:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_014e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0153:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0158:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0162:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0167:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_016c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0171:  ldc.i4.2
    IL_0172:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0177:  dup
    IL_0178:  ldc.i4.0
    IL_0179:  ldtoken    [mscorlib]System.Action
    IL_017e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0183:  ldtoken    [mscorlib]System.Type
    IL_0188:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0192:  stelem.ref
    IL_0193:  dup
    IL_0194:  ldc.i4.1
    IL_0195:  ldarg.0
    IL_0196:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_019b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a5:  stelem.ref
    IL_01a6:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ab:  ldtoken    [mscorlib]System.Action
    IL_01b0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_01ba:  ldc.i4.0
    IL_01bb:  ldtoken    method bool [mscorlib]System.Delegate::op_Equality(class [mscorlib]System.Delegate,
                                                                            class [mscorlib]System.Delegate)
    IL_01c0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01c5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01ca:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_01cf:  ldc.i4.1
    IL_01d0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01d5:  dup
    IL_01d6:  ldc.i4.0
    IL_01d7:  ldloc.0
    IL_01d8:  stelem.ref
    IL_01d9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01de:  stelem.ref
    IL_01df:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01e4:  ldc.i4.0
    IL_01e5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ea:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01ef:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01f4:  pop
    IL_01f5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_01fa:  ldarg.0
    IL_01fb:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0200:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0205:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020a:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_020f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0214:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0219:  ldc.i4.1
    IL_021a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_021f:  dup
    IL_0220:  ldc.i4.0
    IL_0221:  ldtoken    [mscorlib]System.Action
    IL_0226:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022b:  ldstr      "x"
    IL_0230:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0235:  stloc.0
    IL_0236:  ldloc.0
    IL_0237:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_023c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0241:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0246:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_024b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0250:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0255:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_025a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_025f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0264:  ldc.i4.2
    IL_0265:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_026a:  dup
    IL_026b:  ldc.i4.0
    IL_026c:  ldtoken    [mscorlib]System.Action
    IL_0271:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0276:  ldtoken    [mscorlib]System.Type
    IL_027b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0280:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0285:  stelem.ref
    IL_0286:  dup
    IL_0287:  ldc.i4.1
    IL_0288:  ldarg.0
    IL_0289:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_028e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0293:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0298:  stelem.ref
    IL_0299:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_029e:  ldtoken    [mscorlib]System.Action
    IL_02a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02ad:  ldc.i4.0
    IL_02ae:  ldtoken    method bool [mscorlib]System.Delegate::op_Inequality(class [mscorlib]System.Delegate,
                                                                              class [mscorlib]System.Delegate)
    IL_02b3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02b8:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02bd:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               bool,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_02c2:  ldc.i4.1
    IL_02c3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02c8:  dup
    IL_02c9:  ldc.i4.0
    IL_02ca:  ldloc.0
    IL_02cb:  stelem.ref
    IL_02cc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02d1:  stelem.ref
    IL_02d2:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_02d7:  ldc.i4.0
    IL_02d8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02dd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02e2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_02e7:  pop
    IL_02e8:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_02ed:  ldarg.0
    IL_02ee:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_02f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02fd:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0302:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0307:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_030c:  ldc.i4.1
    IL_030d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0312:  dup
    IL_0313:  ldc.i4.0
    IL_0314:  ldtoken    [mscorlib]System.Int32
    IL_0319:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_031e:  ldstr      "x"
    IL_0323:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0328:  stloc.0
    IL_0329:  ldloc.0
    IL_032a:  ldc.i4.s   37
    IL_032c:  box        [mscorlib]System.Int32
    IL_0331:  ldtoken    [mscorlib]System.Int32
    IL_0336:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0340:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0345:  ldc.i4.1
    IL_0346:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_034b:  dup
    IL_034c:  ldc.i4.0
    IL_034d:  ldloc.0
    IL_034e:  stelem.ref
    IL_034f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0354:  stelem.ref
    IL_0355:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_035a:  ldc.i4.0
    IL_035b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0360:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0365:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_036a:  pop
    IL_036b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0370:  ldarg.0
    IL_0371:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0376:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_037b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0380:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0385:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_038a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_038f:  ldc.i4.1
    IL_0390:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0395:  dup
    IL_0396:  ldc.i4.0
    IL_0397:  ldtoken    [mscorlib]System.Int32
    IL_039c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03a1:  ldstr      "x"
    IL_03a6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03ab:  stloc.0
    IL_03ac:  ldc.i4.1
    IL_03ad:  box        [mscorlib]System.Boolean
    IL_03b2:  ldtoken    [mscorlib]System.Boolean
    IL_03b7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03bc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03c1:  ldc.i4.1
    IL_03c2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03c7:  dup
    IL_03c8:  ldc.i4.0
    IL_03c9:  ldloc.0
    IL_03ca:  stelem.ref
    IL_03cb:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03d0:  stelem.ref
    IL_03d1:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_03d6:  ldc.i4.0
    IL_03d7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03dc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03e1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_03e6:  pop
    IL_03e7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_03ec:  ldarg.0
    IL_03ed:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_03f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03f7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03fc:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0401:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0406:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_040b:  ldc.i4.1
    IL_040c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0411:  dup
    IL_0412:  ldc.i4.0
    IL_0413:  ldtoken    [mscorlib]System.Int32
    IL_0418:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_041d:  ldstr      "x"
    IL_0422:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0427:  stloc.0
    IL_0428:  ldc.i4.1
    IL_0429:  box        [mscorlib]System.Boolean
    IL_042e:  ldtoken    [mscorlib]System.Boolean
    IL_0433:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0438:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_043d:  ldc.i4.1
    IL_043e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0443:  dup
    IL_0444:  ldc.i4.0
    IL_0445:  ldloc.0
    IL_0446:  stelem.ref
    IL_0447:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_044c:  stelem.ref
    IL_044d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0452:  ldc.i4.0
    IL_0453:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0458:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_045d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0462:  pop
    IL_0463:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       290 (0x122)
    .maxstack  12
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [System.Core]System.Linq.Enumerable::SequenceEqual<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldtoken    [mscorlib]System.Double
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  ldc.i4.3
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002d:  dup
    IL_002e:  ldc.i4.0
    IL_002f:  ldc.r8     1.
    IL_0038:  box        [mscorlib]System.Double
    IL_003d:  ldtoken    [mscorlib]System.Double
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  stelem.ref
    IL_004d:  dup
    IL_004e:  ldc.i4.1
    IL_004f:  ldc.r8     2.0099999999999998
    IL_0058:  box        [mscorlib]System.Double
    IL_005d:  ldtoken    [mscorlib]System.Double
    IL_0062:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0067:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006c:  stelem.ref
    IL_006d:  dup
    IL_006e:  ldc.i4.2
    IL_006f:  ldc.r8     3.5
    IL_0078:  box        [mscorlib]System.Double
    IL_007d:  ldtoken    [mscorlib]System.Double
    IL_0082:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0087:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008c:  stelem.ref
    IL_008d:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0092:  stelem.ref
    IL_0093:  dup
    IL_0094:  ldc.i4.1
    IL_0095:  ldtoken    [mscorlib]System.Double
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  ldc.i4.3
    IL_00a0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a5:  dup
    IL_00a6:  ldc.i4.0
    IL_00a7:  ldc.r8     1.
    IL_00b0:  box        [mscorlib]System.Double
    IL_00b5:  ldtoken    [mscorlib]System.Double
    IL_00ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c4:  stelem.ref
    IL_00c5:  dup
    IL_00c6:  ldc.i4.1
    IL_00c7:  ldc.r8     2.0099999999999998
    IL_00d0:  box        [mscorlib]System.Double
    IL_00d5:  ldtoken    [mscorlib]System.Double
    IL_00da:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00df:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e4:  stelem.ref
    IL_00e5:  dup
    IL_00e6:  ldc.i4.2
    IL_00e7:  ldc.r8     3.5
    IL_00f0:  box        [mscorlib]System.Double
    IL_00f5:  ldtoken    [mscorlib]System.Double
    IL_00fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0104:  stelem.ref
    IL_0105:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_010a:  stelem.ref
    IL_010b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0110:  ldc.i4.0
    IL_0111:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0116:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0120:  pop
    IL_0121:  ret
  } // end of method ExpressionTrees::NewArrayAndExtensionMethod

  .method public hidebysig instance void 
          NewMultiDimArray() cil managed
  {
    // Code size       138 (0x8a)
    .maxstack  7
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.2
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  dup
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.3
    IL_0018:  box        [mscorlib]System.Int32
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002c:  stelem.ref
    IL_002d:  dup
    IL_002e:  ldc.i4.1
    IL_002f:  ldc.i4.4
    IL_0030:  box        [mscorlib]System.Int32
    IL_0035:  ldtoken    [mscorlib]System.Int32
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0044:  stelem.ref
    IL_0045:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_004a:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_004f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0054:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0059:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_005e:  ldc.i4.1
    IL_005f:  box        [mscorlib]System.Int32
    IL_0064:  ldtoken    [mscorlib]System.Int32
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0078:  ldc.i4.0
    IL_0079:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0083:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0088:  pop
    IL_0089:  ret
  } // end of method ExpressionTrees::NewMultiDimArray

  .method public hidebysig instance void 
          NewObject() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Object
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldtoken    [mscorlib]System.Object
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0028:  ldc.i4.0
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0033:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0038:  pop
    IL_0039:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       270 (0x10e)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::x
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.3
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::y
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.s   42
    IL_0017:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::z
    IL_001c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0021:  ldloc.0
    IL_0022:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::z
    IL_0036:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_003b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0040:  ldtoken    [mscorlib]System.Int32
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0054:  ldc.i4.0
    IL_0055:  box        [mscorlib]System.Int32
    IL_005a:  ldtoken    [mscorlib]System.Int32
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_006e:  ldc.i4.0
    IL_006f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0074:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0079:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007e:  pop
    IL_007f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0084:  ldloc.0
    IL_0085:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'
    IL_008a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0094:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::y
    IL_0099:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00a8:  ldc.i4.0
    IL_00a9:  box        [mscorlib]System.Int32
    IL_00ae:  ldtoken    [mscorlib]System.Int32
    IL_00b3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00bd:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00c2:  ldc.i4.0
    IL_00c3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00cd:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00d2:  pop
    IL_00d3:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00d8:  ldloc.0
    IL_00d9:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'
    IL_00de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e8:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::x
    IL_00ed:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f2:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00fc:  ldc.i4.0
    IL_00fd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0102:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0107:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_010c:  pop
    IL_010d:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       288 (0x120)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0013:  dup
    IL_0014:  ldc.i4.0
    IL_0015:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_001a:  stfld      class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_001f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0024:  ldtoken    [System.Xml]System.Xml.XmlReaderSettings
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0033:  ldc.i4.2
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0039:  dup
    IL_003a:  ldc.i4.0
    IL_003b:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0040:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0045:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_004a:  ldloc.0
    IL_004b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_005f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0069:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CloseInput()
    IL_006e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0073:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0078:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_007d:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0082:  stelem.ref
    IL_0083:  dup
    IL_0084:  ldc.i4.1
    IL_0085:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_008a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0094:  ldloc.0
    IL_0095:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a4:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_00a9:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00b3:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CheckCharacters()
    IL_00b8:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00bd:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c2:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00cc:  stelem.ref
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00d2:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00d7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00dc:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e1:  ldc.i4.1
    IL_00e2:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e7:  dup
    IL_00e8:  ldc.i4.0
    IL_00e9:  ldloc.0
    IL_00ea:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f9:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_00fe:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0108:  stelem.ref
    IL_0109:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_010e:  ldc.i4.0
    IL_010f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0114:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0119:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_011e:  pop
    IL_011f:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       173 (0xad)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "n"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldtoken    [mscorlib]System.String
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  ldstr      "s"
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldloc.0
    IL_0031:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0036:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0040:  ldc.i4.0
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004b:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_005f:  ldc.i4.2
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0065:  dup
    IL_0066:  ldc.i4.0
    IL_0067:  ldloc.0
    IL_0068:  stelem.ref
    IL_0069:  dup
    IL_006a:  ldc.i4.1
    IL_006b:  ldloc.1
    IL_006c:  stelem.ref
    IL_006d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0072:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0077:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0086:  ldnull
    IL_0087:  ldtoken    [mscorlib]System.Object
    IL_008c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0096:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_009b:  ldc.i4.0
    IL_009c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ab:  pop
    IL_00ac:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       165 (0xa5)
    .maxstack  9
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldnull
    IL_001e:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0023:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0028:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002d:  ldc.i4.0
    IL_002e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0038:  stelem.ref
    IL_0039:  dup
    IL_003a:  ldc.i4.1
    IL_003b:  ldc.i4.1
    IL_003c:  box        [mscorlib]System.Boolean
    IL_0041:  ldtoken    [mscorlib]System.Boolean
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0050:  ldc.i4.0
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0056:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0060:  stelem.ref
    IL_0061:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_006b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0070:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0075:  ldc.i4.1
    IL_0076:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007b:  dup
    IL_007c:  ldc.i4.0
    IL_007d:  ldnull
    IL_007e:  ldtoken    [mscorlib]System.Object
    IL_0083:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008d:  stelem.ref
    IL_008e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0093:  ldc.i4.0
    IL_0094:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0099:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00a3:  pop
    IL_00a4:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       347 (0x15b)
    .maxstack  22
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method !!0 [System.Core]System.Linq.Enumerable::Single<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldnull
    IL_001e:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<string,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0023:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0028:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002d:  ldc.i4.2
    IL_002e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0033:  dup
    IL_0034:  ldc.i4.0
    IL_0035:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  ldc.i4.1
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0045:  dup
    IL_0046:  ldc.i4.0
    IL_0047:  ldtoken    method instance void class '<>f__AnonymousType1`2'<string,string>::.ctor(!0,
                                                                                                  !1)
    IL_004c:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005b:  ldc.i4.2
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0061:  dup
    IL_0062:  ldc.i4.0
    IL_0063:  ldstr      "a"
    IL_0068:  ldtoken    [mscorlib]System.String
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  stelem.ref
    IL_0078:  dup
    IL_0079:  ldc.i4.1
    IL_007a:  ldstr      "b"
    IL_007f:  ldtoken    [mscorlib]System.String
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008e:  stelem.ref
    IL_008f:  ldc.i4.2
    IL_0090:  newarr     [mscorlib]System.Reflection.MemberInfo
    IL_0095:  dup
    IL_0096:  ldc.i4.0
    IL_0097:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_009c:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00a1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ab:  stelem.ref
    IL_00ac:  dup
    IL_00ad:  ldc.i4.1
    IL_00ae:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_00b3:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00b8:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bd:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c2:  stelem.ref
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00c8:  stelem.ref
    IL_00c9:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ce:  stelem.ref
    IL_00cf:  dup
    IL_00d0:  ldc.i4.1
    IL_00d1:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00db:  ldstr      "o"
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e5:  stloc.0
    IL_00e6:  ldloc.0
    IL_00e7:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00ec:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00f1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0100:  ldloc.0
    IL_0101:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_0106:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_010b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0110:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0115:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011a:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_011f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0124:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0129:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_012e:  ldc.i4.1
    IL_012f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0134:  dup
    IL_0135:  ldc.i4.0
    IL_0136:  ldloc.0
    IL_0137:  stelem.ref
    IL_0138:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<string,string>,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_013d:  stelem.ref
    IL_013e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0143:  stelem.ref
    IL_0144:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0149:  ldc.i4.0
    IL_014a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0154:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0159:  pop
    IL_015a:  ret
  } // end of method ExpressionTrees::QuotedWithAnonymous

  .method public hidebysig instance void 
          StaticCall() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.3
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldtoken    [mscorlib]System.Object
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0041:  stelem.ref
    IL_0042:  dup
    IL_0043:  ldc.i4.1
    IL_0044:  ldc.i4.0
    IL_0045:  box        [mscorlib]System.Int32
    IL_004a:  ldtoken    [mscorlib]System.Int32
    IL_004f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0054:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0059:  ldtoken    [mscorlib]System.Object
    IL_005e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0068:  stelem.ref
    IL_0069:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006e:  ldc.i4.0
    IL_006f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0074:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0079:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007e:  pop
    IL_007f:  ret
  } // end of method ExpressionTrees::StaticCall

  .method public hidebysig instance void 
          ThisCall() cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0015:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_001a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_001f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0024:  ldc.i4.1
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002a:  dup
    IL_002b:  ldc.i4.0
    IL_002c:  ldc.i4.3
    IL_002d:  box        [mscorlib]System.Int32
    IL_0032:  ldtoken    [mscorlib]System.Int32
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0041:  ldtoken    [mscorlib]System.Object
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0050:  stelem.ref
    IL_0051:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0056:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006b:  pop
    IL_006c:  ret
  } // end of method ExpressionTrees::ThisCall

  .method public hidebysig instance void 
          ThisExplicit() cil managed
  {
    // Code size       108 (0x6c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldarg.0
    IL_001e:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.3
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  ldtoken    [mscorlib]System.Object
    IL_004a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0054:  stelem.ref
    IL_0055:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_005a:  ldc.i4.0
    IL_005b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0060:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0065:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006a:  pop
    IL_006b:  ret
  } // end of method ExpressionTrees::ThisExplicit

  .method public hidebysig instance void 
          TypedConstant() cil managed
  {
    // Code size       100 (0x64)
    .maxstack  7
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Type
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.2
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  dup
    IL_0016:  ldc.i4.0
    IL_0017:  ldtoken    [mscorlib]System.Int32
    IL_001c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0021:  ldtoken    [mscorlib]System.Type
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  stelem.ref
    IL_0031:  dup
    IL_0032:  ldc.i4.1
    IL_0033:  ldtoken    [mscorlib]System.String
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  ldtoken    [mscorlib]System.Type
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  stelem.ref
    IL_004d:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0062:  pop
    IL_0063:  ret
  } // end of method ExpressionTrees::TypedConstant

  .method public hidebysig instance void 
          StaticCallImplicitCast() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.3
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldtoken    [mscorlib]System.Object
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0041:  stelem.ref
    IL_0042:  dup
    IL_0043:  ldc.i4.1
    IL_0044:  ldc.i4.0
    IL_0045:  box        [mscorlib]System.Int32
    IL_004a:  ldtoken    [mscorlib]System.Int32
    IL_004f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0054:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0059:  ldtoken    [mscorlib]System.Object
    IL_005e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0068:  stelem.ref
    IL_0069:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006e:  ldc.i4.0
    IL_006f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0074:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0079:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007e:  pop
    IL_007f:  ret
  } // end of method ExpressionTrees::StaticCallImplicitCast

  .method public hidebysig instance void 
          StaticMembers() cil managed
  {
    // Code size       216 (0xd8)
    .maxstack  10
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_001a:  ldnull
    IL_001b:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_002f:  ldnull
    IL_0030:  ldtoken    method valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromMilliseconds(float64)
    IL_0035:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003f:  ldc.i4.1
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0045:  dup
    IL_0046:  ldc.i4.0
    IL_0047:  ldc.r8     10.000999999999999
    IL_0050:  box        [mscorlib]System.Double
    IL_0055:  ldtoken    [mscorlib]System.Double
    IL_005a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0064:  stelem.ref
    IL_0065:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006a:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::op_Addition(valuetype [mscorlib]System.DateTime,
                                                                                                           valuetype [mscorlib]System.TimeSpan)
    IL_006f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0074:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0079:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_007e:  ldc.i4.0
    IL_007f:  ldtoken    method bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                               valuetype [mscorlib]System.DateTime)
    IL_0084:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0089:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  bool,
                                                                                                                                                  class [mscorlib]System.Reflection.MethodInfo)
    IL_0093:  ldtoken    method instance string [mscorlib]System.Boolean::ToString()
    IL_0098:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a2:  ldc.i4.0
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ad:  ldstr      "False"
    IL_00b2:  ldtoken    [mscorlib]System.String
    IL_00b7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00c6:  ldc.i4.0
    IL_00c7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00d6:  pop
    IL_00d7:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       406 (0x196)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::i
    IL_000d:  ldloc.0
    IL_000e:  ldstr      "X"
    IL_0013:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001d:  ldstr      "a\n\\b"
    IL_0022:  ldtoken    [mscorlib]System.String
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0041:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_0046:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0050:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0055:  ldloc.0
    IL_0056:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_006a:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0074:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0088:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_008d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0092:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0097:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_009c:  ldc.i4.2
    IL_009d:  box        [mscorlib]System.Int32
    IL_00a2:  ldtoken    [mscorlib]System.Int32
    IL_00a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00b6:  ldc.i4.0
    IL_00b7:  box        [mscorlib]System.Boolean
    IL_00bc:  ldtoken    [mscorlib]System.Boolean
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cb:  ldc.i4.1
    IL_00cc:  box        [mscorlib]System.Boolean
    IL_00d1:  ldtoken    [mscorlib]System.Boolean
    IL_00d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00db:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e0:  ldc.i4.1
    IL_00e1:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00e6:  box        [mscorlib]System.Decimal
    IL_00eb:  ldtoken    [mscorlib]System.Decimal
    IL_00f0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fa:  ldloc.0
    IL_00fb:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'
    IL_0100:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010a:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::i
    IL_010f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0114:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0119:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_011e:  ldtoken    [mscorlib]System.Decimal
    IL_0123:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0128:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_012d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0132:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0137:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_013c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0141:  ldc.i4.0
    IL_0142:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0147:  box        [mscorlib]System.Decimal
    IL_014c:  ldtoken    [mscorlib]System.Decimal
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0160:  ldc.i4.0
    IL_0161:  box        [mscorlib]System.Boolean
    IL_0166:  ldtoken    [mscorlib]System.Boolean
    IL_016b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0170:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0175:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_017a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_017f:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0184:  ldc.i4.0
    IL_0185:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0194:  pop
    IL_0195:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       118 (0x76)
    .maxstack  5
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0014:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_0019:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldtoken    [mscorlib]System.Double
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0037:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0046:  ldtoken    method instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_004b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0064:  ldc.i4.0
    IL_0065:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0074:  pop
    IL_0075:  ret
  } // end of method ExpressionTrees::GenericClassInstance

  .method public hidebysig instance void 
          GenericClassStatic() cil managed
  {
    // Code size       90 (0x5a)
    .maxstack  5
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001a:  ldtoken    [mscorlib]System.Double
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0029:  ldnull
    IL_002a:  ldtoken    method !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_002f:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0034:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0048:  ldc.i4.0
    IL_0049:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0058:  pop
    IL_0059:  ret
  } // end of method ExpressionTrees::GenericClassStatic

  .method public hidebysig instance void 
          InvokeGenericMethod() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.0
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0030:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0035:  pop
    IL_0036:  ret
  } // end of method ExpressionTrees::InvokeGenericMethod

  .method private hidebysig static void  Test<T>(!!T delegateExpression,
                                                 class [System.Core]System.Linq.Expressions.Expression`1<!!T> expressionTree) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method ExpressionTrees::Test

  .method public hidebysig static void  ArrayIndexer() cil managed
  {
    // Code size       603 (0x25b)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_0'(int32[])
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_0'
    IL_001f:  ldtoken    int32[]
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "array"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.0
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  ldtoken    [mscorlib]System.Int32
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_004f:  ldc.i4.1
    IL_0050:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0055:  dup
    IL_0056:  ldc.i4.0
    IL_0057:  ldloc.0
    IL_0058:  stelem.ref
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0063:  ldsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_1'
    IL_0068:  dup
    IL_0069:  brtrue.s   IL_0082

    IL_006b:  pop
    IL_006c:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0071:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_1'(int32[],
                                                                                                                                     int32)
    IL_0077:  newobj     instance void class [mscorlib]System.Func`3<int32[],int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_007c:  dup
    IL_007d:  stsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_1'
    IL_0082:  ldtoken    int32[]
    IL_0087:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008c:  ldstr      "array"
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0096:  stloc.0
    IL_0097:  ldtoken    [mscorlib]System.Int32
    IL_009c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a1:  ldstr      "index"
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ab:  stloc.1
    IL_00ac:  ldloc.0
    IL_00ad:  ldloc.1
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00b3:  ldc.i4.2
    IL_00b4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b9:  dup
    IL_00ba:  ldc.i4.0
    IL_00bb:  ldloc.0
    IL_00bc:  stelem.ref
    IL_00bd:  dup
    IL_00be:  ldc.i4.1
    IL_00bf:  ldloc.1
    IL_00c0:  stelem.ref
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c6:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[],int32,int32>>(!!0,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00cb:  ldsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_2'
    IL_00d0:  dup
    IL_00d1:  brtrue.s   IL_00ea

    IL_00d3:  pop
    IL_00d4:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00d9:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_2'(int32[0...,0...])
    IL_00df:  newobj     instance void class [mscorlib]System.Func`2<int32[0...,0...],int32>::.ctor(object,
                                                                                                    native int)
    IL_00e4:  dup
    IL_00e5:  stsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_2'
    IL_00ea:  ldtoken    int32[0...,0...]
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  ldstr      "array"
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00fe:  stloc.1
    IL_00ff:  ldloc.1
    IL_0100:  ldc.i4.2
    IL_0101:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0106:  dup
    IL_0107:  ldc.i4.0
    IL_0108:  ldc.i4.0
    IL_0109:  box        [mscorlib]System.Int32
    IL_010e:  ldtoken    [mscorlib]System.Int32
    IL_0113:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0118:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011d:  stelem.ref
    IL_011e:  dup
    IL_011f:  ldc.i4.1
    IL_0120:  ldc.i4.5
    IL_0121:  box        [mscorlib]System.Int32
    IL_0126:  ldtoken    [mscorlib]System.Int32
    IL_012b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0130:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0135:  stelem.ref
    IL_0136:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_013b:  ldc.i4.1
    IL_013c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0141:  dup
    IL_0142:  ldc.i4.0
    IL_0143:  ldloc.1
    IL_0144:  stelem.ref
    IL_0145:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_014a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(!!0,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_014f:  ldsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_3'
    IL_0154:  dup
    IL_0155:  brtrue.s   IL_016e

    IL_0157:  pop
    IL_0158:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_015d:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_3'(int32[0...,0...],
                                                                                                                                     int32)
    IL_0163:  newobj     instance void class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>::.ctor(object,
                                                                                                          native int)
    IL_0168:  dup
    IL_0169:  stsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_3'
    IL_016e:  ldtoken    int32[0...,0...]
    IL_0173:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0178:  ldstr      "array"
    IL_017d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0182:  stloc.1
    IL_0183:  ldtoken    [mscorlib]System.Int32
    IL_0188:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018d:  ldstr      "index"
    IL_0192:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0197:  stloc.0
    IL_0198:  ldloc.1
    IL_0199:  ldc.i4.2
    IL_019a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_019f:  dup
    IL_01a0:  ldc.i4.0
    IL_01a1:  ldloc.0
    IL_01a2:  stelem.ref
    IL_01a3:  dup
    IL_01a4:  ldc.i4.1
    IL_01a5:  ldc.i4.7
    IL_01a6:  box        [mscorlib]System.Int32
    IL_01ab:  ldtoken    [mscorlib]System.Int32
    IL_01b0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ba:  stelem.ref
    IL_01bb:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01c0:  ldc.i4.2
    IL_01c1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01c6:  dup
    IL_01c7:  ldc.i4.0
    IL_01c8:  ldloc.1
    IL_01c9:  stelem.ref
    IL_01ca:  dup
    IL_01cb:  ldc.i4.1
    IL_01cc:  ldloc.0
    IL_01cd:  stelem.ref
    IL_01ce:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01d3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(!!0,
                                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01d8:  ldsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_4'
    IL_01dd:  dup
    IL_01de:  brtrue.s   IL_01f7

    IL_01e0:  pop
    IL_01e1:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01e6:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_4'(int32[][],
                                                                                                                                     int32)
    IL_01ec:  newobj     instance void class [mscorlib]System.Func`3<int32[][],int32,int32>::.ctor(object,
                                                                                                   native int)
    IL_01f1:  dup
    IL_01f2:  stsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_4'
    IL_01f7:  ldtoken    int32[][]
    IL_01fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0201:  ldstr      "array"
    IL_0206:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_020b:  stloc.0
    IL_020c:  ldtoken    [mscorlib]System.Int32
    IL_0211:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0216:  ldstr      "index"
    IL_021b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0220:  stloc.1
    IL_0221:  ldloc.0
    IL_0222:  ldloc.1
    IL_0223:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0228:  ldc.i4.7
    IL_0229:  box        [mscorlib]System.Int32
    IL_022e:  ldtoken    [mscorlib]System.Int32
    IL_0233:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0238:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_023d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0242:  ldc.i4.2
    IL_0243:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0248:  dup
    IL_0249:  ldc.i4.0
    IL_024a:  ldloc.0
    IL_024b:  stelem.ref
    IL_024c:  dup
    IL_024d:  ldc.i4.1
    IL_024e:  ldloc.1
    IL_024f:  stelem.ref
    IL_0250:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[][],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0255:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[][],int32,int32>>(!!0,
                                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_025a:  ret
  } // end of method ExpressionTrees::ArrayIndexer

  .method public hidebysig static void  ArrayLength() cil managed
  {
    // Code size       162 (0xa2)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayLength>b__63_0'(int32[])
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_0'
    IL_001f:  ldtoken    int32[]
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "array"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_003a:  ldc.i4.1
    IL_003b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0040:  dup
    IL_0041:  ldc.i4.0
    IL_0042:  ldloc.0
    IL_0043:  stelem.ref
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_1'
    IL_0053:  dup
    IL_0054:  brtrue.s   IL_006d

    IL_0056:  pop
    IL_0057:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_005c:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayLength>b__63_1'()
    IL_0062:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0067:  dup
    IL_0068:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_1'
    IL_006d:  ldnull
    IL_006e:  ldtoken    [mscorlib]System.Array
    IL_0073:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007d:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0082:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0087:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0091:  ldc.i4.0
    IL_0092:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0097:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00a1:  ret
  } // end of method ExpressionTrees::ArrayLength

  .method public hidebysig static void  NewObj() cil managed
  {
    // Code size       545 (0x221)
    .maxstack  7
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_0'
    IL_001f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0039:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_003e:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_1'
    IL_0043:  dup
    IL_0044:  brtrue.s   IL_005d

    IL_0046:  pop
    IL_0047:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_004c:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_1'()
    IL_0052:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0057:  dup
    IL_0058:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_1'
    IL_005d:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
    IL_0062:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0067:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_006c:  ldc.i4.1
    IL_006d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0072:  dup
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4.5
    IL_0075:  box        [mscorlib]System.Int32
    IL_007a:  ldtoken    [mscorlib]System.Int32
    IL_007f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0089:  stelem.ref
    IL_008a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_008f:  ldc.i4.0
    IL_0090:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0095:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_009f:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_2'
    IL_00a4:  dup
    IL_00a5:  brtrue.s   IL_00be

    IL_00a7:  pop
    IL_00a8:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00ad:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_2'()
    IL_00b3:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_00b8:  dup
    IL_00b9:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_2'
    IL_00be:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_00cd:  ldc.i4.0
    IL_00ce:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00dd:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_3'
    IL_00e2:  dup
    IL_00e3:  brtrue.s   IL_00fc

    IL_00e5:  pop
    IL_00e6:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00eb:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_3'()
    IL_00f1:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_00f6:  dup
    IL_00f7:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_3'
    IL_00fc:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
    IL_0101:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0106:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_010b:  ldc.i4.1
    IL_010c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0111:  dup
    IL_0112:  ldc.i4.0
    IL_0113:  ldc.i4.5
    IL_0114:  box        [mscorlib]System.Int32
    IL_0119:  ldtoken    [mscorlib]System.Int32
    IL_011e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0123:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0128:  stelem.ref
    IL_0129:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_012e:  ldc.i4.0
    IL_012f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0134:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0139:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_013e:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_4'
    IL_0143:  dup
    IL_0144:  brtrue.s   IL_015d

    IL_0146:  pop
    IL_0147:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_014c:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_4'()
    IL_0152:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0157:  dup
    IL_0158:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_4'
    IL_015d:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0162:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0167:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_016c:  ldc.i4.0
    IL_016d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0172:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0177:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_017c:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_5'
    IL_0181:  dup
    IL_0182:  brtrue.s   IL_019b

    IL_0184:  pop
    IL_0185:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_018a:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_5'()
    IL_0190:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0195:  dup
    IL_0196:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_5'
    IL_019b:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>
    IL_01a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a5:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_01aa:  ldc.i4.0
    IL_01ab:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01b0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01ba:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_6'
    IL_01bf:  dup
    IL_01c0:  brtrue.s   IL_01d9

    IL_01c2:  pop
    IL_01c3:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01c8:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_6'()
    IL_01ce:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_01d3:  dup
    IL_01d4:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_6'
    IL_01d9:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
    IL_01de:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>
    IL_01e3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e8:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01ed:  ldc.i4.1
    IL_01ee:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01f3:  dup
    IL_01f4:  ldc.i4.0
    IL_01f5:  ldc.i4.5
    IL_01f6:  box        [mscorlib]System.Int32
    IL_01fb:  ldtoken    [mscorlib]System.Int32
    IL_0200:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0205:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020a:  stelem.ref
    IL_020b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0210:  ldc.i4.0
    IL_0211:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0216:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_021b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0220:  ret
  } // end of method ExpressionTrees::NewObj

  .method public hidebysig static void  TypeOfExpr() cil managed
  {
    // Code size       361 (0x169)
    .maxstack  3
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldtoken    [mscorlib]System.Type
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  ldc.i4.0
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0043:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0048:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_1'
    IL_004d:  dup
    IL_004e:  brtrue.s   IL_0067

    IL_0050:  pop
    IL_0051:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0056:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_1'()
    IL_005c:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0061:  dup
    IL_0062:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_1'
    IL_0067:  ldtoken    [mscorlib]System.Object
    IL_006c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0071:  ldtoken    [mscorlib]System.Type
    IL_0076:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0080:  ldc.i4.0
    IL_0081:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0086:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0090:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_2'
    IL_0095:  dup
    IL_0096:  brtrue.s   IL_00af

    IL_0098:  pop
    IL_0099:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_009e:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_2'()
    IL_00a4:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00a9:  dup
    IL_00aa:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_2'
    IL_00af:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_00b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b9:  ldtoken    [mscorlib]System.Type
    IL_00be:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c8:  ldc.i4.0
    IL_00c9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ce:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00d8:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_3'
    IL_00dd:  dup
    IL_00de:  brtrue.s   IL_00f7

    IL_00e0:  pop
    IL_00e1:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00e6:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_3'()
    IL_00ec:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00f1:  dup
    IL_00f2:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_3'
    IL_00f7:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0101:  ldtoken    [mscorlib]System.Type
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0110:  ldc.i4.0
    IL_0111:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0116:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0120:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_4'
    IL_0125:  dup
    IL_0126:  brtrue.s   IL_013f

    IL_0128:  pop
    IL_0129:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_012e:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_4'()
    IL_0134:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0139:  dup
    IL_013a:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_4'
    IL_013f:  ldtoken    int32*
    IL_0144:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0149:  ldtoken    [mscorlib]System.Type
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0158:  ldc.i4.0
    IL_0159:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_015e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0163:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0168:  ret
  } // end of method ExpressionTrees::TypeOfExpr

  .method public hidebysig static void  AsTypeExpr() cil managed
  {
    // Code size       177 (0xb1)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<AsTypeExpr>b__66_0'(object)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>::.ctor(object,
                                                                                                                                                native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_0'
    IL_001f:  ldtoken    [mscorlib]System.Object
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "obj"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_0044:  ldc.i4.1
    IL_0045:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004a:  dup
    IL_004b:  ldc.i4.0
    IL_004c:  ldloc.0
    IL_004d:  stelem.ref
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(!!0,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0058:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_1'
    IL_005d:  dup
    IL_005e:  brtrue.s   IL_0077

    IL_0060:  pop
    IL_0061:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0066:  ldftn      instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<AsTypeExpr>b__66_1'(object)
    IL_006c:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>::.ctor(object,
                                                                                                                                                                               native int)
    IL_0071:  dup
    IL_0072:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_1'
    IL_0077:  ldtoken    [mscorlib]System.Object
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  ldstr      "obj"
    IL_0086:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_008b:  stloc.0
    IL_008c:  ldloc.0
    IL_008d:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
    IL_0092:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0097:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_009c:  ldc.i4.1
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  dup
    IL_00a3:  ldc.i4.0
    IL_00a4:  ldloc.0
    IL_00a5:  stelem.ref
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ab:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(!!0,
                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b0:  ret
  } // end of method ExpressionTrees::AsTypeExpr

  .method public hidebysig static void  IsTypeExpr() cil managed
  {
    // Code size       89 (0x59)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__67_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<IsTypeExpr>b__67_0'(object)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<object,bool>::.ctor(object,
                                                                                         native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__67_0'
    IL_001f:  ldtoken    [mscorlib]System.Object
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "obj"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0044:  ldc.i4.1
    IL_0045:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004a:  dup
    IL_004b:  ldc.i4.0
    IL_004c:  ldloc.0
    IL_004d:  stelem.ref
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,bool>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0058:  ret
  } // end of method ExpressionTrees::IsTypeExpr

  .method public hidebysig static void  UnaryLogicalOperators() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__68_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<UnaryLogicalOperators>b__68_0'(bool)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<bool,bool>::.ctor(object,
                                                                                       native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__68_0'
    IL_001f:  ldtoken    [mscorlib]System.Boolean
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "a"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003a:  ldc.i4.1
    IL_003b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0040:  dup
    IL_0041:  ldc.i4.0
    IL_0042:  ldloc.0
    IL_0043:  stelem.ref
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<bool,bool>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  ret
  } // end of method ExpressionTrees::UnaryLogicalOperators

  .method public hidebysig static void  ConditionalOperator() cil managed
  {
    // Code size       157 (0x9d)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldnull
    IL_0001:  ldtoken    [mscorlib]System.Boolean
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldstr      "a"
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.5
    IL_0018:  box        [mscorlib]System.Int32
    IL_001d:  ldtoken    [mscorlib]System.Int32
    IL_0022:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0027:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002c:  ldc.i4.s   10
    IL_002e:  box        [mscorlib]System.Int32
    IL_0033:  ldtoken    [mscorlib]System.Int32
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0047:  ldc.i4.1
    IL_0048:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004d:  dup
    IL_004e:  ldc.i4.0
    IL_004f:  ldloc.0
    IL_0050:  stelem.ref
    IL_0051:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0056:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,int32>(object,
                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_005b:  pop
    IL_005c:  ldnull
    IL_005d:  ldtoken    [mscorlib]System.Object
    IL_0062:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0067:  ldstr      "a"
    IL_006c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0071:  stloc.0
    IL_0072:  ldloc.0
    IL_0073:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0078:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007d:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0087:  ldc.i4.1
    IL_0088:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008d:  dup
    IL_008e:  ldc.i4.0
    IL_008f:  ldloc.0
    IL_0090:  stelem.ref
    IL_0091:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0096:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<object,object>(object,
                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_009b:  pop
    IL_009c:  ret
  } // end of method ExpressionTrees::ConditionalOperator

  .method public hidebysig static void  BinaryLogicalOperators() cil managed
  {
    // Code size       1604 (0x644)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  ldnull
    IL_0001:  ldtoken    [mscorlib]System.Int32
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldstr      "a"
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0015:  stloc.0
    IL_0016:  ldtoken    [mscorlib]System.Int32
    IL_001b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0020:  ldstr      "b"
    IL_0025:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002a:  stloc.1
    IL_002b:  ldloc.0
    IL_002c:  ldloc.1
    IL_002d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0032:  ldc.i4.2
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  dup
    IL_0039:  ldc.i4.0
    IL_003a:  ldloc.0
    IL_003b:  stelem.ref
    IL_003c:  dup
    IL_003d:  ldc.i4.1
    IL_003e:  ldloc.1
    IL_003f:  stelem.ref
    IL_0040:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0045:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_004a:  pop
    IL_004b:  ldnull
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  ldstr      "a"
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0060:  stloc.1
    IL_0061:  ldtoken    [mscorlib]System.Int32
    IL_0066:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  ldstr      "b"
    IL_0070:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0075:  stloc.0
    IL_0076:  ldloc.1
    IL_0077:  ldloc.0
    IL_0078:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_007d:  ldc.i4.2
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0083:  dup
    IL_0084:  ldc.i4.0
    IL_0085:  ldloc.1
    IL_0086:  stelem.ref
    IL_0087:  dup
    IL_0088:  ldc.i4.1
    IL_0089:  ldloc.0
    IL_008a:  stelem.ref
    IL_008b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0090:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0095:  pop
    IL_0096:  ldnull
    IL_0097:  ldtoken    [mscorlib]System.Int32
    IL_009c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a1:  ldstr      "a"
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ab:  stloc.0
    IL_00ac:  ldtoken    [mscorlib]System.Int32
    IL_00b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b6:  ldstr      "b"
    IL_00bb:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00c0:  stloc.1
    IL_00c1:  ldloc.0
    IL_00c2:  ldloc.1
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00c8:  ldc.i4.2
    IL_00c9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ce:  dup
    IL_00cf:  ldc.i4.0
    IL_00d0:  ldloc.0
    IL_00d1:  stelem.ref
    IL_00d2:  dup
    IL_00d3:  ldc.i4.1
    IL_00d4:  ldloc.1
    IL_00d5:  stelem.ref
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00db:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_00e0:  pop
    IL_00e1:  ldnull
    IL_00e2:  ldtoken    [mscorlib]System.Int32
    IL_00e7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ec:  ldstr      "a"
    IL_00f1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f6:  stloc.1
    IL_00f7:  ldtoken    [mscorlib]System.Int32
    IL_00fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0101:  ldstr      "b"
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010b:  stloc.0
    IL_010c:  ldloc.1
    IL_010d:  ldloc.0
    IL_010e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0113:  ldc.i4.2
    IL_0114:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0119:  dup
    IL_011a:  ldc.i4.0
    IL_011b:  ldloc.1
    IL_011c:  stelem.ref
    IL_011d:  dup
    IL_011e:  ldc.i4.1
    IL_011f:  ldloc.0
    IL_0120:  stelem.ref
    IL_0121:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0126:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_012b:  pop
    IL_012c:  ldnull
    IL_012d:  ldtoken    [mscorlib]System.Int32
    IL_0132:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0137:  ldstr      "a"
    IL_013c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0141:  stloc.0
    IL_0142:  ldtoken    [mscorlib]System.Int32
    IL_0147:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014c:  ldstr      "b"
    IL_0151:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0156:  stloc.1
    IL_0157:  ldloc.0
    IL_0158:  ldloc.1
    IL_0159:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_015e:  ldc.i4.2
    IL_015f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0164:  dup
    IL_0165:  ldc.i4.0
    IL_0166:  ldloc.0
    IL_0167:  stelem.ref
    IL_0168:  dup
    IL_0169:  ldc.i4.1
    IL_016a:  ldloc.1
    IL_016b:  stelem.ref
    IL_016c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0171:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0176:  pop
    IL_0177:  ldnull
    IL_0178:  ldtoken    [mscorlib]System.Int32
    IL_017d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0182:  ldstr      "a"
    IL_0187:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_018c:  stloc.1
    IL_018d:  ldtoken    [mscorlib]System.Int32
    IL_0192:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0197:  ldstr      "b"
    IL_019c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01a1:  stloc.0
    IL_01a2:  ldloc.1
    IL_01a3:  ldloc.0
    IL_01a4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_01a9:  ldc.i4.2
    IL_01aa:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01af:  dup
    IL_01b0:  ldc.i4.0
    IL_01b1:  ldloc.1
    IL_01b2:  stelem.ref
    IL_01b3:  dup
    IL_01b4:  ldc.i4.1
    IL_01b5:  ldloc.0
    IL_01b6:  stelem.ref
    IL_01b7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01bc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_01c1:  pop
    IL_01c2:  ldnull
    IL_01c3:  ldtoken    [mscorlib]System.Int32
    IL_01c8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01cd:  ldstr      "a"
    IL_01d2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d7:  stloc.0
    IL_01d8:  ldtoken    [mscorlib]System.Int32
    IL_01dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e2:  ldstr      "b"
    IL_01e7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01ec:  stloc.1
    IL_01ed:  ldloc.0
    IL_01ee:  ldc.i4.1
    IL_01ef:  box        [mscorlib]System.Int32
    IL_01f4:  ldtoken    [mscorlib]System.Int32
    IL_01f9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01fe:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0203:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0208:  ldloc.1
    IL_0209:  ldc.i4.2
    IL_020a:  box        [mscorlib]System.Int32
    IL_020f:  ldtoken    [mscorlib]System.Int32
    IL_0214:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0219:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_021e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0223:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0228:  ldc.i4.2
    IL_0229:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_022e:  dup
    IL_022f:  ldc.i4.0
    IL_0230:  ldloc.0
    IL_0231:  stelem.ref
    IL_0232:  dup
    IL_0233:  ldc.i4.1
    IL_0234:  ldloc.1
    IL_0235:  stelem.ref
    IL_0236:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_023b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0240:  pop
    IL_0241:  ldnull
    IL_0242:  ldtoken    [mscorlib]System.Int32
    IL_0247:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_024c:  ldstr      "a"
    IL_0251:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0256:  stloc.1
    IL_0257:  ldtoken    [mscorlib]System.Int32
    IL_025c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0261:  ldstr      "b"
    IL_0266:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_026b:  stloc.0
    IL_026c:  ldloc.1
    IL_026d:  ldc.i4.1
    IL_026e:  box        [mscorlib]System.Int32
    IL_0273:  ldtoken    [mscorlib]System.Int32
    IL_0278:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_027d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0282:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0287:  ldloc.0
    IL_0288:  ldc.i4.2
    IL_0289:  box        [mscorlib]System.Int32
    IL_028e:  ldtoken    [mscorlib]System.Int32
    IL_0293:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0298:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_029d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_02a2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_02a7:  ldc.i4.2
    IL_02a8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02ad:  dup
    IL_02ae:  ldc.i4.0
    IL_02af:  ldloc.1
    IL_02b0:  stelem.ref
    IL_02b1:  dup
    IL_02b2:  ldc.i4.1
    IL_02b3:  ldloc.0
    IL_02b4:  stelem.ref
    IL_02b5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02ba:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_02bf:  pop
    IL_02c0:  ldnull
    IL_02c1:  ldtoken    [mscorlib]System.Int32
    IL_02c6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02cb:  ldstr      "a"
    IL_02d0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02d5:  stloc.0
    IL_02d6:  ldtoken    [mscorlib]System.Int16
    IL_02db:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02e0:  ldstr      "b"
    IL_02e5:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02ea:  stloc.1
    IL_02eb:  ldloc.0
    IL_02ec:  ldloc.1
    IL_02ed:  ldtoken    [mscorlib]System.Int32
    IL_02f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f7:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02fc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0301:  ldc.i4.2
    IL_0302:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0307:  dup
    IL_0308:  ldc.i4.0
    IL_0309:  ldloc.0
    IL_030a:  stelem.ref
    IL_030b:  dup
    IL_030c:  ldc.i4.1
    IL_030d:  ldloc.1
    IL_030e:  stelem.ref
    IL_030f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0314:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int16,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0319:  pop
    IL_031a:  ldnull
    IL_031b:  ldtoken    [mscorlib]System.UInt16
    IL_0320:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0325:  ldstr      "a"
    IL_032a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_032f:  stloc.1
    IL_0330:  ldtoken    [mscorlib]System.Int32
    IL_0335:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033a:  ldstr      "b"
    IL_033f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0344:  stloc.0
    IL_0345:  ldloc.1
    IL_0346:  ldtoken    [mscorlib]System.Int32
    IL_034b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0350:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0355:  ldloc.0
    IL_0356:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_035b:  ldc.i4.2
    IL_035c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0361:  dup
    IL_0362:  ldc.i4.0
    IL_0363:  ldloc.1
    IL_0364:  stelem.ref
    IL_0365:  dup
    IL_0366:  ldc.i4.1
    IL_0367:  ldloc.0
    IL_0368:  stelem.ref
    IL_0369:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint16,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_036e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint16,int32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0373:  pop
    IL_0374:  ldnull
    IL_0375:  ldtoken    [mscorlib]System.Int32
    IL_037a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_037f:  ldstr      "a"
    IL_0384:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0389:  stloc.0
    IL_038a:  ldtoken    [mscorlib]System.Int64
    IL_038f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0394:  ldstr      "b"
    IL_0399:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_039e:  stloc.1
    IL_039f:  ldloc.0
    IL_03a0:  ldtoken    [mscorlib]System.Int64
    IL_03a5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03aa:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03af:  ldloc.1
    IL_03b0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_03b5:  ldc.i4.2
    IL_03b6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03bb:  dup
    IL_03bc:  ldc.i4.0
    IL_03bd:  ldloc.0
    IL_03be:  stelem.ref
    IL_03bf:  dup
    IL_03c0:  ldc.i4.1
    IL_03c1:  ldloc.1
    IL_03c2:  stelem.ref
    IL_03c3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03c8:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_03cd:  pop
    IL_03ce:  ldnull
    IL_03cf:  ldtoken    [mscorlib]System.UInt64
    IL_03d4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03d9:  ldstr      "a"
    IL_03de:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03e3:  stloc.1
    IL_03e4:  ldtoken    [mscorlib]System.UInt32
    IL_03e9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ee:  ldstr      "b"
    IL_03f3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03f8:  stloc.0
    IL_03f9:  ldloc.1
    IL_03fa:  ldloc.0
    IL_03fb:  ldtoken    [mscorlib]System.UInt64
    IL_0400:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0405:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_040a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_040f:  ldc.i4.2
    IL_0410:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0415:  dup
    IL_0416:  ldc.i4.0
    IL_0417:  ldloc.1
    IL_0418:  stelem.ref
    IL_0419:  dup
    IL_041a:  ldc.i4.1
    IL_041b:  ldloc.0
    IL_041c:  stelem.ref
    IL_041d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint64,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0422:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint64,uint32,bool>(object,
                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0427:  pop
    IL_0428:  ldnull
    IL_0429:  ldtoken    [mscorlib]System.Int32
    IL_042e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0433:  ldstr      "a"
    IL_0438:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_043d:  stloc.0
    IL_043e:  ldtoken    [mscorlib]System.UInt32
    IL_0443:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0448:  ldstr      "b"
    IL_044d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0452:  stloc.1
    IL_0453:  ldloc.0
    IL_0454:  ldtoken    [mscorlib]System.Int64
    IL_0459:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_045e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0463:  ldloc.1
    IL_0464:  ldtoken    [mscorlib]System.Int64
    IL_0469:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_046e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0473:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0478:  ldc.i4.2
    IL_0479:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_047e:  dup
    IL_047f:  ldc.i4.0
    IL_0480:  ldloc.0
    IL_0481:  stelem.ref
    IL_0482:  dup
    IL_0483:  ldc.i4.1
    IL_0484:  ldloc.1
    IL_0485:  stelem.ref
    IL_0486:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_048b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,uint32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0490:  pop
    IL_0491:  ldnull
    IL_0492:  ldtoken    [mscorlib]System.Int32
    IL_0497:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_049c:  ldstr      "a"
    IL_04a1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04a6:  stloc.1
    IL_04a7:  ldtoken    [mscorlib]System.Int64
    IL_04ac:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04b1:  ldstr      "b"
    IL_04b6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04bb:  stloc.0
    IL_04bc:  ldloc.1
    IL_04bd:  ldtoken    [mscorlib]System.Int64
    IL_04c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04c7:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04cc:  ldloc.0
    IL_04cd:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_04d2:  ldc.i4.2
    IL_04d3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04d8:  dup
    IL_04d9:  ldc.i4.0
    IL_04da:  ldloc.1
    IL_04db:  stelem.ref
    IL_04dc:  dup
    IL_04dd:  ldc.i4.1
    IL_04de:  ldloc.0
    IL_04df:  stelem.ref
    IL_04e0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04e5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_04ea:  pop
    IL_04eb:  ldnull
    IL_04ec:  ldtoken    [mscorlib]System.Int16
    IL_04f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04f6:  ldstr      "a"
    IL_04fb:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0500:  stloc.0
    IL_0501:  ldtoken    [mscorlib]System.Int64
    IL_0506:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_050b:  ldstr      "b"
    IL_0510:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0515:  stloc.1
    IL_0516:  ldloc.0
    IL_0517:  ldtoken    [mscorlib]System.Int64
    IL_051c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0521:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0526:  ldloc.1
    IL_0527:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_052c:  ldc.i4.2
    IL_052d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0532:  dup
    IL_0533:  ldc.i4.0
    IL_0534:  ldloc.0
    IL_0535:  stelem.ref
    IL_0536:  dup
    IL_0537:  ldc.i4.1
    IL_0538:  ldloc.1
    IL_0539:  stelem.ref
    IL_053a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_053f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int16,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0544:  pop
    IL_0545:  ldnull
    IL_0546:  ldtoken    [mscorlib]System.Int32
    IL_054b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0550:  ldstr      "a"
    IL_0555:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_055a:  stloc.1
    IL_055b:  ldtoken    [mscorlib]System.Int32
    IL_0560:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0565:  ldstr      "b"
    IL_056a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_056f:  stloc.0
    IL_0570:  ldloc.1
    IL_0571:  ldc.i4.1
    IL_0572:  box        [mscorlib]System.Int32
    IL_0577:  ldtoken    [mscorlib]System.Int32
    IL_057c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0581:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0586:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_058b:  ldloc.0
    IL_058c:  ldc.i4.2
    IL_058d:  box        [mscorlib]System.Int32
    IL_0592:  ldtoken    [mscorlib]System.Int32
    IL_0597:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_059c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_05a1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_05a6:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_05ab:  ldc.i4.2
    IL_05ac:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05b1:  dup
    IL_05b2:  ldc.i4.0
    IL_05b3:  ldloc.1
    IL_05b4:  stelem.ref
    IL_05b5:  dup
    IL_05b6:  ldc.i4.1
    IL_05b7:  ldloc.0
    IL_05b8:  stelem.ref
    IL_05b9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05be:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_05c3:  pop
    IL_05c4:  ldnull
    IL_05c5:  ldtoken    [mscorlib]System.Int32
    IL_05ca:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05cf:  ldstr      "a"
    IL_05d4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05d9:  stloc.0
    IL_05da:  ldtoken    [mscorlib]System.Int32
    IL_05df:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05e4:  ldstr      "b"
    IL_05e9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05ee:  stloc.1
    IL_05ef:  ldloc.0
    IL_05f0:  ldc.i4.1
    IL_05f1:  box        [mscorlib]System.Int32
    IL_05f6:  ldtoken    [mscorlib]System.Int32
    IL_05fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0600:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0605:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_060a:  ldloc.1
    IL_060b:  ldc.i4.2
    IL_060c:  box        [mscorlib]System.Int32
    IL_0611:  ldtoken    [mscorlib]System.Int32
    IL_0616:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_061b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0620:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0625:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_062a:  ldc.i4.2
    IL_062b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0630:  dup
    IL_0631:  ldc.i4.0
    IL_0632:  ldloc.0
    IL_0633:  stelem.ref
    IL_0634:  dup
    IL_0635:  ldc.i4.1
    IL_0636:  ldloc.1
    IL_0637:  stelem.ref
    IL_0638:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_063d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0642:  pop
    IL_0643:  ret
  } // end of method ExpressionTrees::BinaryLogicalOperators

  .method public hidebysig static void  UnaryArithmeticOperators() cil managed
  {
    // Code size       152 (0x98)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<UnaryArithmeticOperators>b__71_0'(int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "a"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  dup
    IL_003c:  ldc.i4.0
    IL_003d:  ldloc.0
    IL_003e:  stelem.ref
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0049:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_1'
    IL_004e:  dup
    IL_004f:  brtrue.s   IL_0068

    IL_0051:  pop
    IL_0052:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0057:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<UnaryArithmeticOperators>b__71_1'(int32)
    IL_005d:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0062:  dup
    IL_0063:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_1'
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  ldstr      "a"
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007c:  stloc.0
    IL_007d:  ldloc.0
    IL_007e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0083:  ldc.i4.1
    IL_0084:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0089:  dup
    IL_008a:  ldc.i4.0
    IL_008b:  ldloc.0
    IL_008c:  stelem.ref
    IL_008d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0092:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0097:  ret
  } // end of method ExpressionTrees::UnaryArithmeticOperators

  .method public hidebysig static void  BinaryArithmeticOperators() cil managed
  {
    // Code size       1711 (0x6af)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_0'(int32,
                                                                                                                                                  int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "a"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldtoken    [mscorlib]System.Int32
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  ldstr      "b"
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0048:  stloc.1
    IL_0049:  ldloc.0
    IL_004a:  ldloc.1
    IL_004b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0050:  ldc.i4.2
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0056:  dup
    IL_0057:  ldc.i4.0
    IL_0058:  ldloc.0
    IL_0059:  stelem.ref
    IL_005a:  dup
    IL_005b:  ldc.i4.1
    IL_005c:  ldloc.1
    IL_005d:  stelem.ref
    IL_005e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0063:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0068:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_1'
    IL_006d:  dup
    IL_006e:  brtrue.s   IL_0087

    IL_0070:  pop
    IL_0071:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0076:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_1'(int32,
                                                                                                                                                  int32)
    IL_007c:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0081:  dup
    IL_0082:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_1'
    IL_0087:  ldtoken    [mscorlib]System.Int32
    IL_008c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  ldstr      "a"
    IL_0096:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_009b:  stloc.1
    IL_009c:  ldtoken    [mscorlib]System.Int32
    IL_00a1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a6:  ldstr      "b"
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00b0:  stloc.0
    IL_00b1:  ldloc.1
    IL_00b2:  ldloc.0
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00b8:  ldc.i4.2
    IL_00b9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00be:  dup
    IL_00bf:  ldc.i4.0
    IL_00c0:  ldloc.1
    IL_00c1:  stelem.ref
    IL_00c2:  dup
    IL_00c3:  ldc.i4.1
    IL_00c4:  ldloc.0
    IL_00c5:  stelem.ref
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00cb:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00d0:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_2'
    IL_00d5:  dup
    IL_00d6:  brtrue.s   IL_00ef

    IL_00d8:  pop
    IL_00d9:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00de:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_2'(int32,
                                                                                                                                                  int32)
    IL_00e4:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00e9:  dup
    IL_00ea:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_2'
    IL_00ef:  ldtoken    [mscorlib]System.Int32
    IL_00f4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f9:  ldstr      "a"
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0103:  stloc.0
    IL_0104:  ldtoken    [mscorlib]System.Int32
    IL_0109:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010e:  ldstr      "b"
    IL_0113:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0118:  stloc.1
    IL_0119:  ldloc.0
    IL_011a:  ldloc.1
    IL_011b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0120:  ldc.i4.2
    IL_0121:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0126:  dup
    IL_0127:  ldc.i4.0
    IL_0128:  ldloc.0
    IL_0129:  stelem.ref
    IL_012a:  dup
    IL_012b:  ldc.i4.1
    IL_012c:  ldloc.1
    IL_012d:  stelem.ref
    IL_012e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0133:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0138:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_3'
    IL_013d:  dup
    IL_013e:  brtrue.s   IL_0157

    IL_0140:  pop
    IL_0141:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0146:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_3'(int32,
                                                                                                                                                  int32)
    IL_014c:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0151:  dup
    IL_0152:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_3'
    IL_0157:  ldtoken    [mscorlib]System.Int32
    IL_015c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0161:  ldstr      "a"
    IL_0166:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_016b:  stloc.1
    IL_016c:  ldtoken    [mscorlib]System.Int32
    IL_0171:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0176:  ldstr      "b"
    IL_017b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0180:  stloc.0
    IL_0181:  ldloc.1
    IL_0182:  ldloc.0
    IL_0183:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0188:  ldc.i4.2
    IL_0189:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018e:  dup
    IL_018f:  ldc.i4.0
    IL_0190:  ldloc.1
    IL_0191:  stelem.ref
    IL_0192:  dup
    IL_0193:  ldc.i4.1
    IL_0194:  ldloc.0
    IL_0195:  stelem.ref
    IL_0196:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_019b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01a0:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_4'
    IL_01a5:  dup
    IL_01a6:  brtrue.s   IL_01bf

    IL_01a8:  pop
    IL_01a9:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01ae:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_4'(int32,
                                                                                                                                                  int32)
    IL_01b4:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_01b9:  dup
    IL_01ba:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_4'
    IL_01bf:  ldtoken    [mscorlib]System.Int32
    IL_01c4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c9:  ldstr      "a"
    IL_01ce:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d3:  stloc.0
    IL_01d4:  ldtoken    [mscorlib]System.Int32
    IL_01d9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01de:  ldstr      "b"
    IL_01e3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e8:  stloc.1
    IL_01e9:  ldloc.0
    IL_01ea:  ldloc.1
    IL_01eb:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_01f0:  ldc.i4.2
    IL_01f1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01f6:  dup
    IL_01f7:  ldc.i4.0
    IL_01f8:  ldloc.0
    IL_01f9:  stelem.ref
    IL_01fa:  dup
    IL_01fb:  ldc.i4.1
    IL_01fc:  ldloc.1
    IL_01fd:  stelem.ref
    IL_01fe:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0203:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0208:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_5'
    IL_020d:  dup
    IL_020e:  brtrue.s   IL_0227

    IL_0210:  pop
    IL_0211:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0216:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_5'(int64,
                                                                                                                                                  int32)
    IL_021c:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0221:  dup
    IL_0222:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_5'
    IL_0227:  ldtoken    [mscorlib]System.Int64
    IL_022c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0231:  ldstr      "a"
    IL_0236:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_023b:  stloc.1
    IL_023c:  ldtoken    [mscorlib]System.Int32
    IL_0241:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0246:  ldstr      "b"
    IL_024b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0250:  stloc.0
    IL_0251:  ldloc.1
    IL_0252:  ldloc.0
    IL_0253:  ldtoken    [mscorlib]System.Int64
    IL_0258:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0262:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0267:  ldc.i4.2
    IL_0268:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_026d:  dup
    IL_026e:  ldc.i4.0
    IL_026f:  ldloc.1
    IL_0270:  stelem.ref
    IL_0271:  dup
    IL_0272:  ldc.i4.1
    IL_0273:  ldloc.0
    IL_0274:  stelem.ref
    IL_0275:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_027a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_027f:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_6'
    IL_0284:  dup
    IL_0285:  brtrue.s   IL_029e

    IL_0287:  pop
    IL_0288:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_028d:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_6'(int64,
                                                                                                                                                  int32)
    IL_0293:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0298:  dup
    IL_0299:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_6'
    IL_029e:  ldtoken    [mscorlib]System.Int64
    IL_02a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a8:  ldstr      "a"
    IL_02ad:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02b2:  stloc.0
    IL_02b3:  ldtoken    [mscorlib]System.Int32
    IL_02b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02bd:  ldstr      "b"
    IL_02c2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02c7:  stloc.1
    IL_02c8:  ldloc.0
    IL_02c9:  ldloc.1
    IL_02ca:  ldtoken    [mscorlib]System.Int64
    IL_02cf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02d4:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02d9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_02de:  ldc.i4.2
    IL_02df:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02e4:  dup
    IL_02e5:  ldc.i4.0
    IL_02e6:  ldloc.0
    IL_02e7:  stelem.ref
    IL_02e8:  dup
    IL_02e9:  ldc.i4.1
    IL_02ea:  ldloc.1
    IL_02eb:  stelem.ref
    IL_02ec:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02f1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_02f6:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_7'
    IL_02fb:  dup
    IL_02fc:  brtrue.s   IL_0315

    IL_02fe:  pop
    IL_02ff:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0304:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_7'(int64,
                                                                                                                                                  int32)
    IL_030a:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_030f:  dup
    IL_0310:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_7'
    IL_0315:  ldtoken    [mscorlib]System.Int64
    IL_031a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_031f:  ldstr      "a"
    IL_0324:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0329:  stloc.1
    IL_032a:  ldtoken    [mscorlib]System.Int32
    IL_032f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0334:  ldstr      "b"
    IL_0339:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_033e:  stloc.0
    IL_033f:  ldloc.1
    IL_0340:  ldloc.0
    IL_0341:  ldtoken    [mscorlib]System.Int64
    IL_0346:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0350:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0355:  ldc.i4.2
    IL_0356:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_035b:  dup
    IL_035c:  ldc.i4.0
    IL_035d:  ldloc.1
    IL_035e:  stelem.ref
    IL_035f:  dup
    IL_0360:  ldc.i4.1
    IL_0361:  ldloc.0
    IL_0362:  stelem.ref
    IL_0363:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0368:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_036d:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_8'
    IL_0372:  dup
    IL_0373:  brtrue.s   IL_038c

    IL_0375:  pop
    IL_0376:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_037b:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_8'(int64,
                                                                                                                                                  int32)
    IL_0381:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0386:  dup
    IL_0387:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_8'
    IL_038c:  ldtoken    [mscorlib]System.Int64
    IL_0391:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0396:  ldstr      "a"
    IL_039b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03a0:  stloc.0
    IL_03a1:  ldtoken    [mscorlib]System.Int32
    IL_03a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ab:  ldstr      "b"
    IL_03b0:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03b5:  stloc.1
    IL_03b6:  ldloc.0
    IL_03b7:  ldloc.1
    IL_03b8:  ldtoken    [mscorlib]System.Int64
    IL_03bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03c2:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03c7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_03cc:  ldc.i4.2
    IL_03cd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03d2:  dup
    IL_03d3:  ldc.i4.0
    IL_03d4:  ldloc.0
    IL_03d5:  stelem.ref
    IL_03d6:  dup
    IL_03d7:  ldc.i4.1
    IL_03d8:  ldloc.1
    IL_03d9:  stelem.ref
    IL_03da:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03df:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_03e4:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_9'
    IL_03e9:  dup
    IL_03ea:  brtrue.s   IL_0403

    IL_03ec:  pop
    IL_03ed:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_03f2:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_9'(int64,
                                                                                                                                                  int32)
    IL_03f8:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_03fd:  dup
    IL_03fe:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_9'
    IL_0403:  ldtoken    [mscorlib]System.Int64
    IL_0408:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_040d:  ldstr      "a"
    IL_0412:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0417:  stloc.1
    IL_0418:  ldtoken    [mscorlib]System.Int32
    IL_041d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0422:  ldstr      "b"
    IL_0427:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_042c:  stloc.0
    IL_042d:  ldloc.1
    IL_042e:  ldloc.0
    IL_042f:  ldtoken    [mscorlib]System.Int64
    IL_0434:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0439:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_043e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0443:  ldc.i4.2
    IL_0444:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0449:  dup
    IL_044a:  ldc.i4.0
    IL_044b:  ldloc.1
    IL_044c:  stelem.ref
    IL_044d:  dup
    IL_044e:  ldc.i4.1
    IL_044f:  ldloc.0
    IL_0450:  stelem.ref
    IL_0451:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0456:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_045b:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_10'
    IL_0460:  dup
    IL_0461:  brtrue.s   IL_047a

    IL_0463:  pop
    IL_0464:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0469:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_10'(int16,
                                                                                                                                                   int32)
    IL_046f:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0474:  dup
    IL_0475:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_10'
    IL_047a:  ldtoken    [mscorlib]System.Int16
    IL_047f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0484:  ldstr      "a"
    IL_0489:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_048e:  stloc.0
    IL_048f:  ldtoken    [mscorlib]System.Int32
    IL_0494:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0499:  ldstr      "b"
    IL_049e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04a3:  stloc.1
    IL_04a4:  ldloc.0
    IL_04a5:  ldtoken    [mscorlib]System.Int32
    IL_04aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04af:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04b4:  ldloc.1
    IL_04b5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_04ba:  ldc.i4.2
    IL_04bb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04c0:  dup
    IL_04c1:  ldc.i4.0
    IL_04c2:  ldloc.0
    IL_04c3:  stelem.ref
    IL_04c4:  dup
    IL_04c5:  ldc.i4.1
    IL_04c6:  ldloc.1
    IL_04c7:  stelem.ref
    IL_04c8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04cd:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_04d2:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_11'
    IL_04d7:  dup
    IL_04d8:  brtrue.s   IL_04f1

    IL_04da:  pop
    IL_04db:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_04e0:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_11'(int32,
                                                                                                                                                   int16)
    IL_04e6:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_04eb:  dup
    IL_04ec:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_11'
    IL_04f1:  ldtoken    [mscorlib]System.Int32
    IL_04f6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04fb:  ldstr      "a"
    IL_0500:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0505:  stloc.1
    IL_0506:  ldtoken    [mscorlib]System.Int16
    IL_050b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0510:  ldstr      "b"
    IL_0515:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_051a:  stloc.0
    IL_051b:  ldloc.1
    IL_051c:  ldloc.0
    IL_051d:  ldtoken    [mscorlib]System.Int32
    IL_0522:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0527:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_052c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0531:  ldc.i4.2
    IL_0532:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0537:  dup
    IL_0538:  ldc.i4.0
    IL_0539:  ldloc.1
    IL_053a:  stelem.ref
    IL_053b:  dup
    IL_053c:  ldc.i4.1
    IL_053d:  ldloc.0
    IL_053e:  stelem.ref
    IL_053f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0544:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0549:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_12'
    IL_054e:  dup
    IL_054f:  brtrue.s   IL_0568

    IL_0551:  pop
    IL_0552:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0557:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_12'(int16,
                                                                                                                                                   int32)
    IL_055d:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0562:  dup
    IL_0563:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_12'
    IL_0568:  ldtoken    [mscorlib]System.Int16
    IL_056d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0572:  ldstr      "a"
    IL_0577:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_057c:  stloc.0
    IL_057d:  ldtoken    [mscorlib]System.Int32
    IL_0582:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0587:  ldstr      "b"
    IL_058c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0591:  stloc.1
    IL_0592:  ldloc.0
    IL_0593:  ldtoken    [mscorlib]System.Int32
    IL_0598:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_059d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_05a2:  ldloc.1
    IL_05a3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_05a8:  ldc.i4.2
    IL_05a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05ae:  dup
    IL_05af:  ldc.i4.0
    IL_05b0:  ldloc.0
    IL_05b1:  stelem.ref
    IL_05b2:  dup
    IL_05b3:  ldc.i4.1
    IL_05b4:  ldloc.1
    IL_05b5:  stelem.ref
    IL_05b6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05bb:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_05c0:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_13'
    IL_05c5:  dup
    IL_05c6:  brtrue.s   IL_05df

    IL_05c8:  pop
    IL_05c9:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_05ce:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_13'(int32,
                                                                                                                                                   int16)
    IL_05d4:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_05d9:  dup
    IL_05da:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_13'
    IL_05df:  ldtoken    [mscorlib]System.Int32
    IL_05e4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05e9:  ldstr      "a"
    IL_05ee:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05f3:  stloc.1
    IL_05f4:  ldtoken    [mscorlib]System.Int16
    IL_05f9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05fe:  ldstr      "b"
    IL_0603:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0608:  stloc.0
    IL_0609:  ldloc.1
    IL_060a:  ldloc.0
    IL_060b:  ldtoken    [mscorlib]System.Int32
    IL_0610:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0615:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_061a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_061f:  ldc.i4.2
    IL_0620:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0625:  dup
    IL_0626:  ldc.i4.0
    IL_0627:  ldloc.1
    IL_0628:  stelem.ref
    IL_0629:  dup
    IL_062a:  ldc.i4.1
    IL_062b:  ldloc.0
    IL_062c:  stelem.ref
    IL_062d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0632:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0637:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_14'
    IL_063c:  dup
    IL_063d:  brtrue.s   IL_0656

    IL_063f:  pop
    IL_0640:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0645:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_14'(int16,
                                                                                                                                                   int32)
    IL_064b:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0650:  dup
    IL_0651:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_14'
    IL_0656:  ldtoken    [mscorlib]System.Int16
    IL_065b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0660:  ldstr      "a"
    IL_0665:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_066a:  stloc.0
    IL_066b:  ldtoken    [mscorlib]System.Int32
    IL_0670:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0675:  ldstr      "b"
    IL_067a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_067f:  stloc.1
    IL_0680:  ldloc.0
    IL_0681:  ldtoken    [mscorlib]System.Int32
    IL_0686:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_068b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0690:  ldloc.1
    IL_0691:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_0696:  ldc.i4.2
    IL_0697:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_069c:  dup
    IL_069d:  ldc.i4.0
    IL_069e:  ldloc.0
    IL_069f:  stelem.ref
    IL_06a0:  dup
    IL_06a1:  ldc.i4.1
    IL_06a2:  ldloc.1
    IL_06a3:  stelem.ref
    IL_06a4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_06a9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_06ae:  ret
  } // end of method ExpressionTrees::BinaryArithmeticOperators

  .method public hidebysig static void  BitOperators() cil managed
  {
    // Code size       391 (0x187)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_0'(int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "a"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003a:  ldc.i4.1
    IL_003b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0040:  dup
    IL_0041:  ldc.i4.0
    IL_0042:  ldloc.0
    IL_0043:  stelem.ref
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004e:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_1'
    IL_0053:  dup
    IL_0054:  brtrue.s   IL_006d

    IL_0056:  pop
    IL_0057:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_005c:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_1'(int32,
                                                                                                                                     int32)
    IL_0062:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0067:  dup
    IL_0068:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_1'
    IL_006d:  ldtoken    [mscorlib]System.Int32
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  ldstr      "a"
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0081:  stloc.0
    IL_0082:  ldtoken    [mscorlib]System.Int32
    IL_0087:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008c:  ldstr      "b"
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0096:  stloc.1
    IL_0097:  ldloc.0
    IL_0098:  ldloc.1
    IL_0099:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::And(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_009e:  ldc.i4.2
    IL_009f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a4:  dup
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldloc.0
    IL_00a7:  stelem.ref
    IL_00a8:  dup
    IL_00a9:  ldc.i4.1
    IL_00aa:  ldloc.1
    IL_00ab:  stelem.ref
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b6:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_2'
    IL_00bb:  dup
    IL_00bc:  brtrue.s   IL_00d5

    IL_00be:  pop
    IL_00bf:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00c4:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_2'(int32,
                                                                                                                                     int32)
    IL_00ca:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00cf:  dup
    IL_00d0:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_2'
    IL_00d5:  ldtoken    [mscorlib]System.Int32
    IL_00da:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00df:  ldstr      "a"
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e9:  stloc.1
    IL_00ea:  ldtoken    [mscorlib]System.Int32
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  ldstr      "b"
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00fe:  stloc.0
    IL_00ff:  ldloc.1
    IL_0100:  ldloc.0
    IL_0101:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Or(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_0106:  ldc.i4.2
    IL_0107:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010c:  dup
    IL_010d:  ldc.i4.0
    IL_010e:  ldloc.1
    IL_010f:  stelem.ref
    IL_0110:  dup
    IL_0111:  ldc.i4.1
    IL_0112:  ldloc.0
    IL_0113:  stelem.ref
    IL_0114:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0119:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_011e:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_3'
    IL_0123:  dup
    IL_0124:  brtrue.s   IL_013d

    IL_0126:  pop
    IL_0127:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_012c:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_3'(int32,
                                                                                                                                     int32)
    IL_0132:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0137:  dup
    IL_0138:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_3'
    IL_013d:  ldtoken    [mscorlib]System.Int32
    IL_0142:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0147:  ldstr      "a"
    IL_014c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0151:  stloc.0
    IL_0152:  ldtoken    [mscorlib]System.Int32
    IL_0157:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015c:  ldstr      "b"
    IL_0161:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0166:  stloc.1
    IL_0167:  ldloc.0
    IL_0168:  ldloc.1
    IL_0169:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ExclusiveOr(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_016e:  ldc.i4.2
    IL_016f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0174:  dup
    IL_0175:  ldc.i4.0
    IL_0176:  ldloc.0
    IL_0177:  stelem.ref
    IL_0178:  dup
    IL_0179:  ldc.i4.1
    IL_017a:  ldloc.1
    IL_017b:  stelem.ref
    IL_017c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0181:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0186:  ret
  } // end of method ExpressionTrees::BitOperators

  .method public hidebysig static void  ShiftOperators() cil managed
  {
    // Code size       397 (0x18d)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_0'(int32)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "a"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.2
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  ldtoken    [mscorlib]System.Int32
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_004f:  ldc.i4.1
    IL_0050:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0055:  dup
    IL_0056:  ldc.i4.0
    IL_0057:  ldloc.0
    IL_0058:  stelem.ref
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0063:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_1'
    IL_0068:  dup
    IL_0069:  brtrue.s   IL_0082

    IL_006b:  pop
    IL_006c:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0071:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_1'(int32)
    IL_0077:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_007c:  dup
    IL_007d:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_1'
    IL_0082:  ldtoken    [mscorlib]System.Int32
    IL_0087:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008c:  ldstr      "a"
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0096:  stloc.0
    IL_0097:  ldloc.0
    IL_0098:  ldc.i4.2
    IL_0099:  box        [mscorlib]System.Int32
    IL_009e:  ldtoken    [mscorlib]System.Int32
    IL_00a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_00b2:  ldc.i4.1
    IL_00b3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b8:  dup
    IL_00b9:  ldc.i4.0
    IL_00ba:  ldloc.0
    IL_00bb:  stelem.ref
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00c6:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_2'
    IL_00cb:  dup
    IL_00cc:  brtrue.s   IL_00e5

    IL_00ce:  pop
    IL_00cf:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00d4:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_2'(int64)
    IL_00da:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_00df:  dup
    IL_00e0:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_2'
    IL_00e5:  ldtoken    [mscorlib]System.Int64
    IL_00ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ef:  ldstr      "a"
    IL_00f4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f9:  stloc.0
    IL_00fa:  ldloc.0
    IL_00fb:  ldc.i4.2
    IL_00fc:  box        [mscorlib]System.Int32
    IL_0101:  ldtoken    [mscorlib]System.Int32
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0110:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0115:  ldc.i4.1
    IL_0116:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011b:  dup
    IL_011c:  ldc.i4.0
    IL_011d:  ldloc.0
    IL_011e:  stelem.ref
    IL_011f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0124:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0129:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_3'
    IL_012e:  dup
    IL_012f:  brtrue.s   IL_0148

    IL_0131:  pop
    IL_0132:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0137:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_3'(int64)
    IL_013d:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_0142:  dup
    IL_0143:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_3'
    IL_0148:  ldtoken    [mscorlib]System.Int64
    IL_014d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0152:  ldstr      "a"
    IL_0157:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_015c:  stloc.0
    IL_015d:  ldloc.0
    IL_015e:  ldc.i4.2
    IL_015f:  box        [mscorlib]System.Int32
    IL_0164:  ldtoken    [mscorlib]System.Int32
    IL_0169:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0173:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_0178:  ldc.i4.1
    IL_0179:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_017e:  dup
    IL_017f:  ldc.i4.0
    IL_0180:  ldloc.0
    IL_0181:  stelem.ref
    IL_0182:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0187:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_018c:  ret
  } // end of method ExpressionTrees::ShiftOperators

  .method public hidebysig static void  SimpleExpressions() cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<SimpleExpressions>b__75_0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_0'
    IL_001f:  ldc.i4.0
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  ldc.i4.0
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0044:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_1'
    IL_0049:  dup
    IL_004a:  brtrue.s   IL_0063

    IL_004c:  pop
    IL_004d:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0052:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<SimpleExpressions>b__75_1'(int32)
    IL_0058:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_005d:  dup
    IL_005e:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_1'
    IL_0063:  ldtoken    [mscorlib]System.Int32
    IL_0068:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006d:  ldstr      "a"
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0077:  stloc.0
    IL_0078:  ldloc.0
    IL_0079:  ldc.i4.1
    IL_007a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007f:  dup
    IL_0080:  ldc.i4.0
    IL_0081:  ldloc.0
    IL_0082:  stelem.ref
    IL_0083:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0088:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008d:  ret
  } // end of method ExpressionTrees::SimpleExpressions

  .method public hidebysig static void  Capturing() cil managed
  {
    // Code size       73 (0x49)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.5
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::captured
    IL_000d:  ldloc.0
    IL_000e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::'<Capturing>b__0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0019:  ldloc.0
    IL_001a:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0029:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::captured
    IL_002e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0038:  ldc.i4.0
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0043:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0048:  ret
  } // end of method ExpressionTrees::Capturing

  .method public hidebysig static void  FieldAndPropertyAccess() cil managed
  {
    // Code size       432 (0x1b0)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldnull
    IL_0001:  ldc.i4.1
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  ldtoken    [mscorlib]System.Int32
    IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0016:  ldc.i4.0
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_001c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0021:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0026:  pop
    IL_0027:  ldnull
    IL_0028:  ldnull
    IL_0029:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticField
    IL_002e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0038:  ldc.i4.0
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0043:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0048:  pop
    IL_0049:  ldnull
    IL_004a:  ldnull
    IL_004b:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticReadonlyField
    IL_0050:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_005a:  ldc.i4.0
    IL_005b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0060:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0065:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006a:  pop
    IL_006b:  ldnull
    IL_006c:  ldnull
    IL_006d:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticProperty()
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0081:  ldc.i4.0
    IL_0082:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0091:  pop
    IL_0092:  ldnull
    IL_0093:  ldnull
    IL_0094:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticReadonlyProperty()
    IL_0099:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a8:  ldc.i4.0
    IL_00a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b3:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b8:  pop
    IL_00b9:  ldnull
    IL_00ba:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c4:  ldstr      "a"
    IL_00c9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ce:  stloc.0
    IL_00cf:  ldloc.0
    IL_00d0:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_00d5:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00da:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00df:  ldc.i4.1
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  dup
    IL_00e6:  ldc.i4.0
    IL_00e7:  ldloc.0
    IL_00e8:  stelem.ref
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ee:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_00f3:  pop
    IL_00f4:  ldnull
    IL_00f5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ff:  ldstr      "a"
    IL_0104:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0109:  stloc.0
    IL_010a:  ldloc.0
    IL_010b:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_Property()
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011f:  ldc.i4.1
    IL_0120:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0125:  dup
    IL_0126:  ldc.i4.0
    IL_0127:  ldloc.0
    IL_0128:  stelem.ref
    IL_0129:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0133:  pop
    IL_0134:  ldnull
    IL_0135:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_013a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013f:  ldstr      "a"
    IL_0144:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0149:  stloc.0
    IL_014a:  ldloc.0
    IL_014b:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0150:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0155:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_015a:  ldc.i4.1
    IL_015b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0160:  dup
    IL_0161:  ldc.i4.0
    IL_0162:  ldloc.0
    IL_0163:  stelem.ref
    IL_0164:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0169:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_016e:  pop
    IL_016f:  ldnull
    IL_0170:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0175:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017a:  ldstr      "a"
    IL_017f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0184:  stloc.0
    IL_0185:  ldloc.0
    IL_0186:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_ReadonlyProperty()
    IL_018b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0190:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0195:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_019a:  ldc.i4.1
    IL_019b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01a0:  dup
    IL_01a1:  ldc.i4.0
    IL_01a2:  ldloc.0
    IL_01a3:  stelem.ref
    IL_01a4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01a9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_01ae:  pop
    IL_01af:  ret
  } // end of method ExpressionTrees::FieldAndPropertyAccess

  .method public hidebysig static void  Call() cil managed
  {
    // Code size       522 (0x20a)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  ldnull
    IL_0001:  ldtoken    [mscorlib]System.String
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  ldstr      "a"
    IL_0010:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0015:  stloc.0
    IL_0016:  ldnull
    IL_0017:  ldtoken    method void [mscorlib]System.Console::WriteLine(string)
    IL_001c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0021:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0026:  ldc.i4.1
    IL_0027:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002c:  dup
    IL_002d:  ldc.i4.0
    IL_002e:  ldloc.0
    IL_002f:  stelem.ref
    IL_0030:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0035:  ldc.i4.1
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  dup
    IL_003c:  ldc.i4.0
    IL_003d:  ldloc.0
    IL_003e:  stelem.ref
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Action`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                       class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Action`1<!!0>>)
    IL_0049:  pop
    IL_004a:  ldsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_0'
    IL_004f:  dup
    IL_0050:  brtrue.s   IL_0069

    IL_0052:  pop
    IL_0053:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0058:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_0'(string)
    IL_005e:  newobj     instance void class [mscorlib]System.Func`2<string,string>::.ctor(object,
                                                                                           native int)
    IL_0063:  dup
    IL_0064:  stsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_0'
    IL_0069:  ldtoken    [mscorlib]System.String
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  ldstr      "a"
    IL_0078:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007d:  stloc.0
    IL_007e:  ldloc.0
    IL_007f:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0084:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0089:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008e:  ldc.i4.0
    IL_008f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0094:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0099:  ldc.i4.1
    IL_009a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009f:  dup
    IL_00a0:  ldc.i4.0
    IL_00a1:  ldloc.0
    IL_00a2:  stelem.ref
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,string>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ad:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_1'
    IL_00b2:  dup
    IL_00b3:  brtrue.s   IL_00cc

    IL_00b5:  pop
    IL_00b6:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00bb:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_1'(int32)
    IL_00c1:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_00c6:  dup
    IL_00c7:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_1'
    IL_00cc:  ldtoken    [mscorlib]System.Int32
    IL_00d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d6:  ldstr      "a"
    IL_00db:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e0:  stloc.0
    IL_00e1:  ldloc.0
    IL_00e2:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_00e7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ec:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00f1:  ldc.i4.0
    IL_00f2:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fc:  ldc.i4.1
    IL_00fd:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0102:  dup
    IL_0103:  ldc.i4.0
    IL_0104:  ldloc.0
    IL_0105:  stelem.ref
    IL_0106:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,string>>(!!0,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0110:  ldsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_2'
    IL_0115:  dup
    IL_0116:  brtrue.s   IL_012f

    IL_0118:  pop
    IL_0119:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_011e:  ldftn      instance char[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_2'(string)
    IL_0124:  newobj     instance void class [mscorlib]System.Func`2<string,char[]>::.ctor(object,
                                                                                           native int)
    IL_0129:  dup
    IL_012a:  stsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_2'
    IL_012f:  ldtoken    [mscorlib]System.String
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  ldstr      "a"
    IL_013e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0143:  stloc.0
    IL_0144:  ldnull
    IL_0145:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_014a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_014f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0154:  ldc.i4.1
    IL_0155:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015a:  dup
    IL_015b:  ldc.i4.0
    IL_015c:  ldloc.0
    IL_015d:  stelem.ref
    IL_015e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0163:  ldc.i4.1
    IL_0164:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0169:  dup
    IL_016a:  ldc.i4.0
    IL_016b:  ldloc.0
    IL_016c:  stelem.ref
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,char[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,char[]>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0177:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_3'
    IL_017c:  dup
    IL_017d:  brtrue.s   IL_0196

    IL_017f:  pop
    IL_0180:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0185:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_3'()
    IL_018b:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_0190:  dup
    IL_0191:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_3'
    IL_0196:  ldc.i4.s   97
    IL_0198:  box        [mscorlib]System.Char
    IL_019d:  ldtoken    [mscorlib]System.Char
    IL_01a2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ac:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_01b1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01b6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01bb:  ldc.i4.1
    IL_01bc:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01c1:  dup
    IL_01c2:  ldc.i4.0
    IL_01c3:  ldc.i4.s   98
    IL_01c5:  box        [mscorlib]System.Char
    IL_01ca:  ldtoken    [mscorlib]System.Char
    IL_01cf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d9:  stelem.ref
    IL_01da:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01df:  ldc.i4.0
    IL_01e0:  box        [mscorlib]System.Int32
    IL_01e5:  ldtoken    [mscorlib]System.Int32
    IL_01ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ef:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_01f9:  ldc.i4.0
    IL_01fa:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ff:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0204:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0209:  ret
  } // end of method ExpressionTrees::Call

  .method public hidebysig static void  Quote() cil managed
  {
    // Code size       198 (0xc6)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__79_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Quote>b__79_0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__79_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldstr      "n"
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0033:  stloc.0
    IL_0034:  ldtoken    [mscorlib]System.String
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  ldstr      "s"
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldloc.0
    IL_004b:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  ldc.i4.0
    IL_005b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0060:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0065:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_006a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0074:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0079:  ldc.i4.2
    IL_007a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007f:  dup
    IL_0080:  ldc.i4.0
    IL_0081:  ldloc.0
    IL_0082:  stelem.ref
    IL_0083:  dup
    IL_0084:  ldc.i4.1
    IL_0085:  ldloc.1
    IL_0086:  stelem.ref
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0091:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a0:  ldnull
    IL_00a1:  ldtoken    [mscorlib]System.Object
    IL_00a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00b5:  ldc.i4.0
    IL_00b6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00bb:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c0:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00c5:  ret
  } // end of method ExpressionTrees::Quote

  .method public hidebysig static void  ArrayInitializer() cil managed
  {
    // Code size       605 (0x25d)
    .maxstack  11
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_0'
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldc.i4.3
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  dup
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.i4.1
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldc.i4.2
    IL_004a:  box        [mscorlib]System.Int32
    IL_004f:  ldtoken    [mscorlib]System.Int32
    IL_0054:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005e:  stelem.ref
    IL_005f:  dup
    IL_0060:  ldc.i4.2
    IL_0061:  ldc.i4.3
    IL_0062:  box        [mscorlib]System.Int32
    IL_0067:  ldtoken    [mscorlib]System.Int32
    IL_006c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0071:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0076:  stelem.ref
    IL_0077:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_007c:  ldc.i4.0
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0082:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0087:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008c:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_1'
    IL_0091:  dup
    IL_0092:  brtrue.s   IL_00ab

    IL_0094:  pop
    IL_0095:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_009a:  ldftn      instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_1'()
    IL_00a0:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_00a5:  dup
    IL_00a6:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_1'
    IL_00ab:  ldtoken    [mscorlib]System.Int32
    IL_00b0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b5:  ldc.i4.1
    IL_00b6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bb:  dup
    IL_00bc:  ldc.i4.0
    IL_00bd:  ldc.i4.3
    IL_00be:  box        [mscorlib]System.Int32
    IL_00c3:  ldtoken    [mscorlib]System.Int32
    IL_00c8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d2:  stelem.ref
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00d8:  ldc.i4.0
    IL_00d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00e8:  ldsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_2'
    IL_00ed:  dup
    IL_00ee:  brtrue.s   IL_0107

    IL_00f0:  pop
    IL_00f1:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00f6:  ldftn      instance int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_2'()
    IL_00fc:  newobj     instance void class [mscorlib]System.Func`1<int32[0...,0...]>::.ctor(object,
                                                                                              native int)
    IL_0101:  dup
    IL_0102:  stsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_2'
    IL_0107:  ldtoken    [mscorlib]System.Int32
    IL_010c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0111:  ldc.i4.2
    IL_0112:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0117:  dup
    IL_0118:  ldc.i4.0
    IL_0119:  ldc.i4.3
    IL_011a:  box        [mscorlib]System.Int32
    IL_011f:  ldtoken    [mscorlib]System.Int32
    IL_0124:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0129:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_012e:  stelem.ref
    IL_012f:  dup
    IL_0130:  ldc.i4.1
    IL_0131:  ldc.i4.5
    IL_0132:  box        [mscorlib]System.Int32
    IL_0137:  ldtoken    [mscorlib]System.Int32
    IL_013c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0141:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0146:  stelem.ref
    IL_0147:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_014c:  ldc.i4.0
    IL_014d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0152:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[0...,0...]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0157:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[0...,0...]>>(!!0,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_015c:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_3'
    IL_0161:  dup
    IL_0162:  brtrue.s   IL_017b

    IL_0164:  pop
    IL_0165:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_016a:  ldftn      instance int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_3'()
    IL_0170:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_0175:  dup
    IL_0176:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_3'
    IL_017b:  ldtoken    int32[]
    IL_0180:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0185:  ldc.i4.1
    IL_0186:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_018b:  dup
    IL_018c:  ldc.i4.0
    IL_018d:  ldc.i4.3
    IL_018e:  box        [mscorlib]System.Int32
    IL_0193:  ldtoken    [mscorlib]System.Int32
    IL_0198:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_019d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a2:  stelem.ref
    IL_01a3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01a8:  ldc.i4.0
    IL_01a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ae:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01b8:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_4'
    IL_01bd:  dup
    IL_01be:  brtrue.s   IL_01d7

    IL_01c0:  pop
    IL_01c1:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01c6:  ldftn      instance int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_4'()
    IL_01cc:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_01d1:  dup
    IL_01d2:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_4'
    IL_01d7:  ldtoken    int32[]
    IL_01dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e1:  ldc.i4.1
    IL_01e2:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e7:  dup
    IL_01e8:  ldc.i4.0
    IL_01e9:  ldtoken    [mscorlib]System.Int32
    IL_01ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f3:  ldc.i4.3
    IL_01f4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01f9:  dup
    IL_01fa:  ldc.i4.0
    IL_01fb:  ldc.i4.1
    IL_01fc:  box        [mscorlib]System.Int32
    IL_0201:  ldtoken    [mscorlib]System.Int32
    IL_0206:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0210:  stelem.ref
    IL_0211:  dup
    IL_0212:  ldc.i4.1
    IL_0213:  ldc.i4.2
    IL_0214:  box        [mscorlib]System.Int32
    IL_0219:  ldtoken    [mscorlib]System.Int32
    IL_021e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0223:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0228:  stelem.ref
    IL_0229:  dup
    IL_022a:  ldc.i4.2
    IL_022b:  ldc.i4.3
    IL_022c:  box        [mscorlib]System.Int32
    IL_0231:  ldtoken    [mscorlib]System.Int32
    IL_0236:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_023b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0240:  stelem.ref
    IL_0241:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0246:  stelem.ref
    IL_0247:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_024c:  ldc.i4.0
    IL_024d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0252:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0257:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_025c:  ret
  } // end of method ExpressionTrees::ArrayInitializer

  .method public hidebysig static void  AnonymousTypes() cil managed
  {
    // Code size       178 (0xb2)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__81_0'
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_001f

    IL_0008:  pop
    IL_0009:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000e:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<AnonymousTypes>b__81_0'()
    IL_0014:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0019:  dup
    IL_001a:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__81_0'
    IL_001f:  ldtoken    method instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_0024:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_0029:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0033:  ldc.i4.2
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0039:  dup
    IL_003a:  ldc.i4.0
    IL_003b:  ldc.i4.5
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  ldtoken    [mscorlib]System.Int32
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0050:  stelem.ref
    IL_0051:  dup
    IL_0052:  ldc.i4.1
    IL_0053:  ldstr      "Test"
    IL_0058:  ldtoken    [mscorlib]System.String
    IL_005d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0067:  stelem.ref
    IL_0068:  ldc.i4.2
    IL_0069:  newarr     [mscorlib]System.Reflection.MemberInfo
    IL_006e:  dup
    IL_006f:  ldc.i4.0
    IL_0070:  ldtoken    method instance !0 class '<>f__AnonymousType2`2'<int32,string>::get_A()
    IL_0075:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  stelem.ref
    IL_0085:  dup
    IL_0086:  ldc.i4.1
    IL_0087:  ldtoken    method instance !1 class '<>f__AnonymousType2`2'<int32,string>::get_B()
    IL_008c:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_0091:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0096:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009b:  stelem.ref
    IL_009c:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00a1:  ldc.i4.0
    IL_00a2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ac:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b1:  ret
  } // end of method ExpressionTrees::AnonymousTypes

  .method public hidebysig static void  ObjectInit() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0016:  dup
    IL_0017:  ldc.i4.0
    IL_0018:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_Property(int32)
    IL_001d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0022:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0027:  ldc.i4.4
    IL_0028:  box        [mscorlib]System.Int32
    IL_002d:  ldtoken    [mscorlib]System.Int32
    IL_0032:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0037:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0041:  stelem.ref
    IL_0042:  dup
    IL_0043:  ldc.i4.1
    IL_0044:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_0049:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004e:  ldc.i4.3
    IL_004f:  box        [mscorlib]System.Int32
    IL_0054:  ldtoken    [mscorlib]System.Int32
    IL_0059:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0068:  stelem.ref
    IL_0069:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_006e:  ldc.i4.0
    IL_006f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0074:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0079:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>(object,
                                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007e:  pop
    IL_007f:  ret
  } // end of method ExpressionTrees::ObjectInit

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ExpressionTrees::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass 
          op_Addition(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass a,
                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass b) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass::.ctor()
    IL_0005:  ret
  } // end of method MyClass::op_Addition

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MyClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
       extends [mscorlib]System.Object
{
  .field public static literal int32 ConstField = int32(0x00000001)
  .field public static initonly int32 StaticReadonlyField
  .field public static int32 StaticField
  .field public initonly int32 ReadonlyField
  .field public int32 Field
  .field private static int32 '<StaticProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 '<Property>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname static 
          int32  get_StaticReadonlyProperty() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method SimpleType::get_StaticReadonlyProperty

  .method public hidebysig specialname static 
          int32  get_StaticProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<StaticProperty>k__BackingField'
    IL_0005:  ret
  } // end of method SimpleType::get_StaticProperty

  .method public hidebysig specialname static 
          void  set_StaticProperty(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<StaticProperty>k__BackingField'
    IL_0006:  ret
  } // end of method SimpleType::set_StaticProperty

  .method public hidebysig specialname instance int32 
          get_ReadonlyProperty() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method SimpleType::get_ReadonlyProperty

  .method public hidebysig specialname instance int32 
          get_Property() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<Property>k__BackingField'
    IL_0006:  ret
  } // end of method SimpleType::get_Property

  .method public hidebysig specialname instance void 
          set_Property(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::'<Property>k__BackingField'
    IL_0007:  ret
  } // end of method SimpleType::set_Property

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.3
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_000e:  ldarg.0
    IL_000f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0014:  ret
  } // end of method SimpleType::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldc.i4.2
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticReadonlyField
    IL_0006:  ldc.i4.3
    IL_0007:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticField
    IL_000c:  ret
  } // end of method SimpleType::.cctor

  .property int32 StaticReadonlyProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticReadonlyProperty()
  } // end of property SimpleType::StaticReadonlyProperty
  .property int32 StaticProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_StaticProperty(int32)
  } // end of property SimpleType::StaticProperty
  .property instance int32 ReadonlyProperty()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_ReadonlyProperty()
  } // end of property SimpleType::ReadonlyProperty
  .property instance int32 Property()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_Property()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_Property(int32)
  } // end of property SimpleType::Property
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SimpleTypeWithCtor::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SimpleTypeWithMultipleCtors::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SimpleTypeWithMultipleCtors::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method GenericClassWithCtor`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method GenericClassWithMultipleCtors`1::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 x) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method GenericClassWithMultipleCtors`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClass`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method GenericClass`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClass`1

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' E429CCA3F703A39CC5954A6572FEC9086135B34E at I_0000BD74
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_0000BD74 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
