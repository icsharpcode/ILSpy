
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ExpressionTrees.dll
// MVID: {6DB33F97-CCA7-4A1C-B8E0-8DA1C4D7C5A0}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02EE0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<X>j__TPar','<A>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 5C 7B 20 58 20 3D 20 7B 58 7D 2C 20 41   // ...\{ X = {X}, A
                                                                                                 20 3D 20 7B 41 7D 20 7D 01 00 54 0E 04 54 79 70   //  = {A} }..T..Typ
                                                                                                 65 10 3C 41 6E 6F 6E 79 6D 6F 75 73 20 54 79 70   // e.<Anonymous Typ
                                                                                                 65 3E )                                           // e>
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
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  ret
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
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 5C 7B 20 58 20 3D 20 7B 58 7D 2C 20 59   // ...\{ X = {X}, Y
                                                                                                 20 3D 20 7B 59 7D 20 7D 01 00 54 0E 04 54 79 70   //  = {Y} }..T..Typ
                                                                                                 65 10 3C 41 6E 6F 6E 79 6D 6F 75 73 20 54 79 70   // e.<Anonymous Typ
                                                                                                 65 3E )                                           // e>
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
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  ret
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
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 5C 7B 20 41 20 3D 20 7B 41 7D 2C 20 42   // ...\{ A = {A}, B
                                                                                                 20 3D 20 7B 42 7D 20 7D 01 00 54 0E 04 54 79 70   //  = {B} }..T..Typ
                                                                                                 65 10 3C 41 6E 6F 6E 79 6D 6F 75 73 20 54 79 70   // e.<Anonymous Typ
                                                                                                 65 3E )                                           // e>
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
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<A>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<A>j__TPar',!'<B>j__TPar'>::'<B>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<B>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  ret
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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field private !X '<InstanceProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (bool V_0)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method GenericClass`1::GenericMethod

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,R>(object x,
                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!T,!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,T2,R>(object x,
                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!T,!!T2,!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::X

  .method public hidebysig instance void 
          Parameter(bool a) cil managed
  {
    // Code size       68 (0x44)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'::a
    IL_000d:  nop
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass7_0'::a
    IL_0028:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0032:  ldc.i4.0
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0042:  pop
    IL_0043:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       68 (0x44)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'::a
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass8_0'::a
    IL_0028:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0032:  ldc.i4.0
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0042:  pop
    IL_0043:  ret
  } // end of method ExpressionTrees::LocalVariable

  .method public hidebysig instance void 
          LambdaParameter() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Boolean
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "a"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.1
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0022:  dup
    IL_0023:  ldc.i4.0
    IL_0024:  ldloc.0
    IL_0025:  stelem.ref
    IL_0026:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,bool>(object,
                                                                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0030:  pop
    IL_0031:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       120 (0x78)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'::x
    IL_000d:  nop
    IL_000e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0013:  ldc.i4.1
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  ldtoken    [mscorlib]System.Int32
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0028:  ldloc.0
    IL_0029:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass10_0'::x
    IL_003d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldc.i4.2
    IL_004d:  box        [mscorlib]System.Int32
    IL_0052:  ldtoken    [mscorlib]System.Int32
    IL_0057:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0066:  ldc.i4.0
    IL_0067:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0071:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0076:  pop
    IL_0077:  ret
  } // end of method ExpressionTrees::AddOperator

  .method public hidebysig instance void 
          AnonymousClasses() cil managed
  {
    // Code size       154 (0x9a)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class '<>f__AnonymousType0`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_000b:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.2
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  dup
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.3
    IL_0023:  box        [mscorlib]System.Int32
    IL_0028:  ldtoken    [mscorlib]System.Int32
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  stelem.ref
    IL_0038:  dup
    IL_0039:  ldc.i4.1
    IL_003a:  ldstr      "a"
    IL_003f:  ldtoken    [mscorlib]System.String
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  ldc.i4.2
    IL_0050:  newarr     [mscorlib]System.Reflection.MemberInfo
    IL_0055:  dup
    IL_0056:  ldc.i4.0
    IL_0057:  ldtoken    method instance !0 class '<>f__AnonymousType0`2'<int32,string>::get_X()
    IL_005c:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0061:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0066:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006b:  stelem.ref
    IL_006c:  dup
    IL_006d:  ldc.i4.1
    IL_006e:  ldtoken    method instance !1 class '<>f__AnonymousType0`2'<int32,string>::get_A()
    IL_0073:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0078:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0082:  stelem.ref
    IL_0083:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_0088:  ldc.i4.0
    IL_0089:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class '<>f__AnonymousType0`2'<int32,string>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0093:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0098:  pop
    IL_0099:  ret
  } // end of method ExpressionTrees::AnonymousClasses

  .method public hidebysig instance void 
          ArrayIndex() cil managed
  {
    // Code size       231 (0xe7)
    .maxstack  7
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.3
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  dup
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.3
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.4
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  dup
    IL_0047:  ldc.i4.2
    IL_0048:  ldc.i4.5
    IL_0049:  box        [mscorlib]System.Int32
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  stelem.ref
    IL_005e:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0063:  ldc.i4.0
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  ldnull
    IL_0079:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_007e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0083:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0088:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008d:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0092:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0097:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a1:  ldc.i4.3
    IL_00a2:  conv.i8
    IL_00a3:  box        [mscorlib]System.Int64
    IL_00a8:  ldtoken    [mscorlib]System.Int64
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_00bc:  ldtoken    [mscorlib]System.Int32
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00cb:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00d5:  ldc.i4.0
    IL_00d6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00db:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e5:  pop
    IL_00e6:  ret
  } // end of method ExpressionTrees::ArrayIndex

  .method public hidebysig instance void 
          ArrayLengthAndDoubles() cil managed
  {
    // Code size       294 (0x126)
    .maxstack  17
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.1
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Concat<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.2
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  dup
    IL_0035:  ldc.i4.0
    IL_0036:  ldtoken    [mscorlib]System.Double
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  ldc.i4.3
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  dup
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.r8     1.
    IL_0051:  box        [mscorlib]System.Double
    IL_0056:  ldtoken    [mscorlib]System.Double
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  stelem.ref
    IL_0066:  dup
    IL_0067:  ldc.i4.1
    IL_0068:  ldc.r8     2.0099999999999998
    IL_0071:  box        [mscorlib]System.Double
    IL_0076:  ldtoken    [mscorlib]System.Double
    IL_007b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0080:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0085:  stelem.ref
    IL_0086:  dup
    IL_0087:  ldc.i4.2
    IL_0088:  ldc.r8     3.5
    IL_0091:  box        [mscorlib]System.Double
    IL_0096:  ldtoken    [mscorlib]System.Double
    IL_009b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a5:  stelem.ref
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ab:  stelem.ref
    IL_00ac:  dup
    IL_00ad:  ldc.i4.1
    IL_00ae:  ldtoken    [mscorlib]System.Double
    IL_00b3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b8:  ldc.i4.2
    IL_00b9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00be:  dup
    IL_00bf:  ldc.i4.0
    IL_00c0:  ldc.r8     1.
    IL_00c9:  box        [mscorlib]System.Double
    IL_00ce:  ldtoken    [mscorlib]System.Double
    IL_00d3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00dd:  stelem.ref
    IL_00de:  dup
    IL_00df:  ldc.i4.1
    IL_00e0:  ldc.r8     2.
    IL_00e9:  box        [mscorlib]System.Double
    IL_00ee:  ldtoken    [mscorlib]System.Double
    IL_00f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fd:  stelem.ref
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0103:  stelem.ref
    IL_0104:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0109:  stelem.ref
    IL_010a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_010f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_0114:  ldc.i4.0
    IL_0115:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0124:  pop
    IL_0125:  ret
  } // end of method ExpressionTrees::ArrayLengthAndDoubles

  .method public hidebysig instance void 
          AsOperator() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Object
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldtoken    [mscorlib]System.String
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_0024:  ldc.i4.0
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0034:  pop
    IL_0035:  ret
  } // end of method ExpressionTrees::AsOperator

  .method public hidebysig instance void 
          ComplexGenericName() cil managed
  {
    // Code size       137 (0x89)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "x"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  box        [mscorlib]System.Int32
    IL_0022:  ldtoken    [mscorlib]System.Int32
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0036:  ldc.i4.1
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003c:  dup
    IL_003d:  ldc.i4.0
    IL_003e:  ldloc.0
    IL_003f:  stelem.ref
    IL_0040:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0045:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_004a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0054:  ldc.i4.1
    IL_0055:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005a:  dup
    IL_005b:  ldc.i4.0
    IL_005c:  ldc.i4.0
    IL_005d:  box        [mscorlib]System.Int32
    IL_0062:  ldtoken    [mscorlib]System.Int32
    IL_0067:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0071:  stelem.ref
    IL_0072:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0077:  ldc.i4.0
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0087:  pop
    IL_0088:  ret
  } // end of method ExpressionTrees::ComplexGenericName

  .method public hidebysig instance void 
          DefaultValue() cil managed
  {
    // Code size       172 (0xac)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.TimeSpan V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.TimeSpan::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.3
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  stelem.ref
    IL_0033:  dup
    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.2
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  ldtoken    [mscorlib]System.Int32
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004a:  stelem.ref
    IL_004b:  dup
    IL_004c:  ldc.i4.2
    IL_004d:  ldc.i4.3
    IL_004e:  box        [mscorlib]System.Int32
    IL_0053:  ldtoken    [mscorlib]System.Int32
    IL_0058:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0062:  stelem.ref
    IL_0063:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0068:  ldloca.s   V_0
    IL_006a:  initobj    [mscorlib]System.TimeSpan
    IL_0070:  ldloc.0
    IL_0071:  box        [mscorlib]System.TimeSpan
    IL_0076:  ldtoken    [mscorlib]System.TimeSpan
    IL_007b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0080:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0085:  ldc.i4.0
    IL_0086:  ldtoken    method bool [mscorlib]System.TimeSpan::op_Equality(valuetype [mscorlib]System.TimeSpan,
                                                                            valuetype [mscorlib]System.TimeSpan)
    IL_008b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0090:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0095:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_009a:  ldc.i4.0
    IL_009b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a5:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00aa:  pop
    IL_00ab:  ret
  } // end of method ExpressionTrees::DefaultValue

  .method public hidebysig instance void 
          EnumConstant() cil managed
  {
    // Code size       104 (0x68)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Object
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_001a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_001f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0024:  ldc.i4.1
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002a:  dup
    IL_002b:  ldc.i4.0
    IL_002c:  ldc.i4.0
    IL_002d:  box        [mscorlib]System.MidpointRounding
    IL_0032:  ldtoken    [mscorlib]System.MidpointRounding
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
    IL_0056:  ldc.i4.0
    IL_0057:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0061:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0066:  pop
    IL_0067:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       191 (0xbf)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ldc.i4.s   20
    IL_000b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_0010:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__18_0'
    IL_0015:  dup
    IL_0016:  brtrue.s   IL_002f

    IL_0018:  pop
    IL_0019:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_001e:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<IndexerAccess>b__18_0'(int32)
    IL_0024:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0029:  dup
    IL_002a:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__18_0'
    IL_002f:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0034:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'::dict
    IL_0039:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_003e:  ldloc.0
    IL_003f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass18_0'::dict
    IL_0053:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_005d:  ldtoken    method instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
    IL_0062:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>
    IL_0067:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0071:  ldc.i4.1
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0077:  dup
    IL_0078:  ldc.i4.0
    IL_0079:  ldstr      "3"
    IL_007e:  ldtoken    [mscorlib]System.String
    IL_0083:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008d:  stelem.ref
    IL_008e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0093:  ldc.i4.3
    IL_0094:  box        [mscorlib]System.Int32
    IL_0099:  ldtoken    [mscorlib]System.Int32
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ad:  ldc.i4.0
    IL_00ae:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b8:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bd:  pop
    IL_00be:  ret
  } // end of method ExpressionTrees::IndexerAccess

  .method public hidebysig instance void 
          IsOperator() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Object
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldtoken    [mscorlib]System.String
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldc.i4.0
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0034:  pop
    IL_0035:  ret
  } // end of method ExpressionTrees::IsOperator

  .method public hidebysig instance void 
          ListInitializer() cil managed
  {
    // Code size       347 (0x15b)
    .maxstack  11
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldc.i4.3
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0022:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0027:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0031:  ldc.i4.2
    IL_0032:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0037:  dup
    IL_0038:  ldc.i4.0
    IL_0039:  ldc.i4.1
    IL_003a:  box        [mscorlib]System.Int32
    IL_003f:  ldtoken    [mscorlib]System.Int32
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  dup
    IL_0050:  ldc.i4.1
    IL_0051:  ldc.i4.1
    IL_0052:  box        [mscorlib]System.Int32
    IL_0057:  ldtoken    [mscorlib]System.Int32
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  stelem.ref
    IL_0067:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_006c:  stelem.ref
    IL_006d:  dup
    IL_006e:  ldc.i4.1
    IL_006f:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0074:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  ldc.i4.2
    IL_0084:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0089:  dup
    IL_008a:  ldc.i4.0
    IL_008b:  ldc.i4.2
    IL_008c:  box        [mscorlib]System.Int32
    IL_0091:  ldtoken    [mscorlib]System.Int32
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a0:  stelem.ref
    IL_00a1:  dup
    IL_00a2:  ldc.i4.1
    IL_00a3:  ldc.i4.2
    IL_00a4:  box        [mscorlib]System.Int32
    IL_00a9:  ldtoken    [mscorlib]System.Int32
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  stelem.ref
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00be:  stelem.ref
    IL_00bf:  dup
    IL_00c0:  ldc.i4.2
    IL_00c1:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_00c6:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_00cb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d5:  ldc.i4.2
    IL_00d6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00db:  dup
    IL_00dc:  ldc.i4.0
    IL_00dd:  ldc.i4.3
    IL_00de:  box        [mscorlib]System.Int32
    IL_00e3:  ldtoken    [mscorlib]System.Int32
    IL_00e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ed:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f2:  stelem.ref
    IL_00f3:  dup
    IL_00f4:  ldc.i4.1
    IL_00f5:  ldc.i4.4
    IL_00f6:  box        [mscorlib]System.Int32
    IL_00fb:  ldtoken    [mscorlib]System.Int32
    IL_0100:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010a:  stelem.ref
    IL_010b:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0110:  stelem.ref
    IL_0111:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_0116:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::get_Count()
    IL_011b:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0120:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0125:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_012a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012f:  ldc.i4.3
    IL_0130:  box        [mscorlib]System.Int32
    IL_0135:  ldtoken    [mscorlib]System.Int32
    IL_013a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0144:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0149:  ldc.i4.0
    IL_014a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0154:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0159:  pop
    IL_015a:  ret
  } // end of method ExpressionTrees::ListInitializer

  .method public hidebysig instance void 
          ListInitializer2() cil managed
  {
    // Code size       316 (0x13c)
    .maxstack  11
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.1
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  dup
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.s   50
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  stelem.ref
    IL_0039:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_003e:  ldc.i4.3
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_0044:  dup
    IL_0045:  ldc.i4.0
    IL_0046:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_004b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  ldc.i4.1
    IL_005b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0060:  dup
    IL_0061:  ldc.i4.0
    IL_0062:  ldc.i4.1
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  stelem.ref
    IL_0078:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_007d:  stelem.ref
    IL_007e:  dup
    IL_007f:  ldc.i4.1
    IL_0080:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0085:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_008a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0094:  ldc.i4.1
    IL_0095:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009a:  dup
    IL_009b:  ldc.i4.0
    IL_009c:  ldc.i4.2
    IL_009d:  box        [mscorlib]System.Int32
    IL_00a2:  ldtoken    [mscorlib]System.Int32
    IL_00a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b1:  stelem.ref
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b7:  stelem.ref
    IL_00b8:  dup
    IL_00b9:  ldc.i4.2
    IL_00ba:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00bf:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00c4:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c9:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ce:  ldc.i4.1
    IL_00cf:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00d4:  dup
    IL_00d5:  ldc.i4.0
    IL_00d6:  ldc.i4.3
    IL_00d7:  box        [mscorlib]System.Int32
    IL_00dc:  ldtoken    [mscorlib]System.Int32
    IL_00e1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00eb:  stelem.ref
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f1:  stelem.ref
    IL_00f2:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00f7:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00fc:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0101:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0106:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_010b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0110:  ldc.i4.3
    IL_0111:  box        [mscorlib]System.Int32
    IL_0116:  ldtoken    [mscorlib]System.Int32
    IL_011b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0120:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0125:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_012a:  ldc.i4.0
    IL_012b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0130:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0135:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_013a:  pop
    IL_013b:  ret
  } // end of method ExpressionTrees::ListInitializer2

  .method public hidebysig instance void 
          ListInitializer3() cil managed
  {
    // Code size       275 (0x113)
    .maxstack  11
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldc.i4.3
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0022:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0027:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0031:  ldc.i4.1
    IL_0032:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0037:  dup
    IL_0038:  ldc.i4.0
    IL_0039:  ldc.i4.1
    IL_003a:  box        [mscorlib]System.Int32
    IL_003f:  ldtoken    [mscorlib]System.Int32
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0054:  stelem.ref
    IL_0055:  dup
    IL_0056:  ldc.i4.1
    IL_0057:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_005c:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0061:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0066:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006b:  ldc.i4.1
    IL_006c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0071:  dup
    IL_0072:  ldc.i4.0
    IL_0073:  ldc.i4.2
    IL_0074:  box        [mscorlib]System.Int32
    IL_0079:  ldtoken    [mscorlib]System.Int32
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0088:  stelem.ref
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_008e:  stelem.ref
    IL_008f:  dup
    IL_0090:  ldc.i4.2
    IL_0091:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0096:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_009b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a5:  ldc.i4.1
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ab:  dup
    IL_00ac:  ldc.i4.0
    IL_00ad:  ldc.i4.3
    IL_00ae:  box        [mscorlib]System.Int32
    IL_00b3:  ldtoken    [mscorlib]System.Int32
    IL_00b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c2:  stelem.ref
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c8:  stelem.ref
    IL_00c9:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00ce:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00d3:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00d8:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e2:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00e7:  ldc.i4.3
    IL_00e8:  box        [mscorlib]System.Int32
    IL_00ed:  ldtoken    [mscorlib]System.Int32
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0101:  ldc.i4.0
    IL_0102:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0107:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0111:  pop
    IL_0112:  ret
  } // end of method ExpressionTrees::ListInitializer3

  .method public hidebysig instance void 
          LiteralCharAndProperty() cil managed
  {
    // Code size       145 (0x91)
    .maxstack  7
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.String::.ctor(char,
                                                                             int32)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.s   32
    IL_001f:  box        [mscorlib]System.Char
    IL_0024:  ldtoken    [mscorlib]System.Char
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  dup
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.3
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  stelem.ref
    IL_004c:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0051:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0056:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_005b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0060:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0065:  ldc.i4.1
    IL_0066:  box        [mscorlib]System.Int32
    IL_006b:  ldtoken    [mscorlib]System.Int32
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_007f:  ldc.i4.0
    IL_0080:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0085:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008f:  pop
    IL_0090:  ret
  } // end of method ExpressionTrees::LiteralCharAndProperty

  .method public hidebysig instance void 
          CharNoCast() cil managed
  {
    // Code size       136 (0x88)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldstr      "abc"
    IL_000b:  ldtoken    [mscorlib]System.String
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
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
    IL_0047:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005b:  ldc.i4.s   98
    IL_005d:  box        [mscorlib]System.Int32
    IL_0062:  ldtoken    [mscorlib]System.Int32
    IL_0067:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0071:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0076:  ldc.i4.0
    IL_0077:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0081:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0086:  pop
    IL_0087:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       407 (0x197)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_0019:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_0047:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0056:  ldloc.0
    IL_0057:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_006b:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0075:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0089:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_008e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0093:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0098:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_009d:  ldc.i4.2
    IL_009e:  box        [mscorlib]System.Int32
    IL_00a3:  ldtoken    [mscorlib]System.Int32
    IL_00a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00b7:  ldc.i4.0
    IL_00b8:  box        [mscorlib]System.Boolean
    IL_00bd:  ldtoken    [mscorlib]System.Boolean
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldc.i4.1
    IL_00cd:  box        [mscorlib]System.Boolean
    IL_00d2:  ldtoken    [mscorlib]System.Boolean
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e1:  ldc.i4.1
    IL_00e2:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00e7:  box        [mscorlib]System.Decimal
    IL_00ec:  ldtoken    [mscorlib]System.Decimal
    IL_00f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fb:  ldloc.0
    IL_00fc:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'
    IL_0101:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010b:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass25_0'::i
    IL_0110:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0115:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_011a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_011f:  ldtoken    [mscorlib]System.Decimal
    IL_0124:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0129:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_012e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0133:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0138:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_013d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0142:  ldc.i4.0
    IL_0143:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0148:  box        [mscorlib]System.Decimal
    IL_014d:  ldtoken    [mscorlib]System.Decimal
    IL_0152:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  ldc.i4.0
    IL_0162:  box        [mscorlib]System.Boolean
    IL_0167:  ldtoken    [mscorlib]System.Boolean
    IL_016c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0171:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0176:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_017b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0180:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0185:  ldc.i4.0
    IL_0186:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0190:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0195:  pop
    IL_0196:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s   42
    IL_000a:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'::z
    IL_000f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0014:  ldloc.0
    IL_0015:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass26_0'::z
    IL_0029:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0033:  ldtoken    [mscorlib]System.Int32
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0047:  ldc.i4.0
    IL_0048:  box        [mscorlib]System.Int32
    IL_004d:  ldtoken    [mscorlib]System.Int32
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0061:  ldc.i4.0
    IL_0062:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0067:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0071:  pop
    IL_0072:  ret
  } // end of method ExpressionTrees::NotImplicitCast

  .method public hidebysig instance void 
          MembersBuiltin() cil managed
  {
    // Code size       402 (0x192)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldc.i4.s   123
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.2
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0011:  box        [mscorlib]System.Decimal
    IL_0016:  ldtoken    [mscorlib]System.Decimal
    IL_001b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0020:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0025:  ldtoken    method instance string [mscorlib]System.Decimal::ToString()
    IL_002a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0034:  ldc.i4.0
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003f:  ldc.i4.0
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004f:  pop
    IL_0050:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0055:  ldc.i4     0x7fff
    IL_005a:  box        [mscorlib]System.AttributeTargets
    IL_005f:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006e:  ldtoken    method instance bool [mscorlib]System.Enum::HasFlag(class [mscorlib]System.Enum)
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  ldc.i4.1
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0083:  dup
    IL_0084:  ldc.i4.0
    IL_0085:  ldc.i4.1
    IL_0086:  box        [mscorlib]System.AttributeTargets
    IL_008b:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0090:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009a:  ldtoken    [mscorlib]System.Enum
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a9:  stelem.ref
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00af:  ldc.i4.0
    IL_00b0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ba:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bf:  pop
    IL_00c0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c5:  ldstr      "abc"
    IL_00ca:  ldtoken    [mscorlib]System.String
    IL_00cf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d9:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_00de:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ed:  ldc.i4.3
    IL_00ee:  box        [mscorlib]System.Int32
    IL_00f3:  ldtoken    [mscorlib]System.Int32
    IL_00f8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0102:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0107:  ldc.i4.0
    IL_0108:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0112:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0117:  pop
    IL_0118:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_011d:  ldc.i4.s   97
    IL_011f:  box        [mscorlib]System.Char
    IL_0124:  ldtoken    [mscorlib]System.Char
    IL_0129:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0133:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_0138:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_013d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0142:  ldc.i4.1
    IL_0143:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0148:  dup
    IL_0149:  ldc.i4.0
    IL_014a:  ldc.i4.s   98
    IL_014c:  box        [mscorlib]System.Char
    IL_0151:  ldtoken    [mscorlib]System.Char
    IL_0156:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0160:  stelem.ref
    IL_0161:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0166:  ldc.i4.0
    IL_0167:  box        [mscorlib]System.Int32
    IL_016c:  ldtoken    [mscorlib]System.Int32
    IL_0171:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0176:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0180:  ldc.i4.0
    IL_0181:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0186:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0190:  pop
    IL_0191:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       541 (0x21d)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.DateTime V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    [mscorlib]System.DateTime
    IL_000e:  ldloc.0
    IL_000f:  box        [mscorlib]System.DateTime
    IL_0014:  ldtoken    [mscorlib]System.DateTime
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0028:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0032:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0037:  ldc.i4.0
    IL_0038:  conv.i8
    IL_0039:  box        [mscorlib]System.Int64
    IL_003e:  ldtoken    [mscorlib]System.Int64
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
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
    IL_0063:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0068:  ldnull
    IL_0069:  ldtoken    [mscorlib]System.Array
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_007d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0082:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0087:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008c:  ldc.i4.0
    IL_008d:  box        [mscorlib]System.Int32
    IL_0092:  ldtoken    [mscorlib]System.Int32
    IL_0097:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a6:  ldc.i4.0
    IL_00a7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b6:  pop
    IL_00b7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00bc:  ldnull
    IL_00bd:  ldtoken    [mscorlib]System.Type
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_00d1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00d6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00db:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00e0:  ldc.i4.0
    IL_00e1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00eb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00f0:  pop
    IL_00f1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00f6:  ldnull
    IL_00f7:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0101:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0106:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_010b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011f:  ldc.i4.0
    IL_0120:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0125:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012f:  pop
    IL_0130:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0135:  ldnull
    IL_0136:  ldtoken    [mscorlib]System.Array
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0145:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_014a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_014f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0154:  ldc.i4.0
    IL_0155:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_015f:  ldnull
    IL_0160:  ldtoken    [mscorlib]System.Object
    IL_0165:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_016f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0174:  ldc.i4.0
    IL_0175:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_017a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_017f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0184:  pop
    IL_0185:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_018a:  ldnull
    IL_018b:  ldtoken    [mscorlib]System.Type
    IL_0190:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0195:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_019a:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_019f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01a4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01a9:  ldc.i4.1
    IL_01aa:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01af:  dup
    IL_01b0:  ldc.i4.0
    IL_01b1:  ldtoken    [mscorlib]System.Object
    IL_01b6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01bb:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_01c0:  stelem.ref
    IL_01c1:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01c6:  ldc.i4.0
    IL_01c7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01cc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01d1:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01d6:  pop
    IL_01d7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_01dc:  ldnull
    IL_01dd:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_01e2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ec:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_01f1:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_01f6:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01fb:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0200:  ldc.i4.0
    IL_0201:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0206:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_020b:  ldc.i4.0
    IL_020c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0211:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0216:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_021b:  pop
    IL_021c:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       347 (0x15b)
    .maxstack  9
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0016:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::'field'
    IL_001b:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0020:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0025:  ldarg.0
    IL_0026:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_002b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0030:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0035:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::C()
    IL_003a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0044:  ldc.i4.0
    IL_0045:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_004a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0054:  ldc.i4.0
    IL_0055:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0064:  pop
    IL_0065:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_006a:  ldnull
    IL_006b:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_0070:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0075:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007a:  ldc.i4.2
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0080:  dup
    IL_0081:  ldc.i4.0
    IL_0082:  ldarg.0
    IL_0083:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0088:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0092:  stelem.ref
    IL_0093:  dup
    IL_0094:  ldc.i4.1
    IL_0095:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_00a4:  stelem.ref
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00af:  ldc.i4.0
    IL_00b0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ba:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bf:  pop
    IL_00c0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00c5:  ldarg.0
    IL_00c6:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00cb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d5:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_00da:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00df:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e4:  ldc.i4.1
    IL_00e5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ea:  dup
    IL_00eb:  ldc.i4.0
    IL_00ec:  ldarg.0
    IL_00ed:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fc:  stelem.ref
    IL_00fd:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0102:  ldarg.0
    IL_0103:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0108:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0112:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees)
    IL_0117:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_011c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0121:  ldc.i4.1
    IL_0122:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0127:  dup
    IL_0128:  ldc.i4.0
    IL_0129:  ldnull
    IL_012a:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_012f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0134:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0139:  stelem.ref
    IL_013a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_013f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0144:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0149:  ldc.i4.0
    IL_014a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_014f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0154:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0159:  pop
    IL_015a:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees other) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       274 (0x112)
    .maxstack  12
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    method bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0029:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0033:  ldc.i4.2
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0039:  dup
    IL_003a:  ldc.i4.0
    IL_003b:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  ldtoken    [mscorlib]System.Type
    IL_004a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0054:  stelem.ref
    IL_0055:  dup
    IL_0056:  ldc.i4.1
    IL_0057:  ldtoken    [mscorlib]System.Int32
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  ldc.i4.4
    IL_0062:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0067:  dup
    IL_0068:  ldc.i4.0
    IL_0069:  ldc.i4     0x7d0
    IL_006e:  box        [mscorlib]System.Int32
    IL_0073:  ldtoken    [mscorlib]System.Int32
    IL_0078:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0082:  stelem.ref
    IL_0083:  dup
    IL_0084:  ldc.i4.1
    IL_0085:  ldc.i4     0x7d4
    IL_008a:  box        [mscorlib]System.Int32
    IL_008f:  ldtoken    [mscorlib]System.Int32
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  stelem.ref
    IL_009f:  dup
    IL_00a0:  ldc.i4.2
    IL_00a1:  ldc.i4     0x7d8
    IL_00a6:  box        [mscorlib]System.Int32
    IL_00ab:  ldtoken    [mscorlib]System.Int32
    IL_00b0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ba:  stelem.ref
    IL_00bb:  dup
    IL_00bc:  ldc.i4.3
    IL_00bd:  ldc.i4     0x7dc
    IL_00c2:  box        [mscorlib]System.Int32
    IL_00c7:  ldtoken    [mscorlib]System.Int32
    IL_00cc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d6:  stelem.ref
    IL_00d7:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00dc:  ldtoken    class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_00e1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00eb:  stelem.ref
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f1:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_00f6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0100:  ldc.i4.0
    IL_0101:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0106:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0110:  pop
    IL_0111:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       873 (0x369)
    .maxstack  13
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_000c:  ldnull
    IL_000d:  ldtoken    method bool [mscorlib]System.Array::TrueForAll<int32>(!!0[],
                                                                               class [mscorlib]System.Predicate`1<!!0>)
    IL_0012:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0017:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001c:  ldc.i4.2
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0022:  dup
    IL_0023:  ldc.i4.0
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  ldc.i4.4
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  dup
    IL_0035:  ldc.i4.0
    IL_0036:  ldc.i4     0x7d0
    IL_003b:  box        [mscorlib]System.Int32
    IL_0040:  ldtoken    [mscorlib]System.Int32
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  dup
    IL_0051:  ldc.i4.1
    IL_0052:  ldc.i4     0x7d4
    IL_0057:  box        [mscorlib]System.Int32
    IL_005c:  ldtoken    [mscorlib]System.Int32
    IL_0061:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0066:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006b:  stelem.ref
    IL_006c:  dup
    IL_006d:  ldc.i4.2
    IL_006e:  ldc.i4     0x7d8
    IL_0073:  box        [mscorlib]System.Int32
    IL_0078:  ldtoken    [mscorlib]System.Int32
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0087:  stelem.ref
    IL_0088:  dup
    IL_0089:  ldc.i4.3
    IL_008a:  ldc.i4     0x7dc
    IL_008f:  box        [mscorlib]System.Int32
    IL_0094:  ldtoken    [mscorlib]System.Int32
    IL_0099:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a3:  stelem.ref
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a9:  stelem.ref
    IL_00aa:  dup
    IL_00ab:  ldc.i4.1
    IL_00ac:  ldtoken    method bool [mscorlib]System.DateTime::IsLeapYear(int32)
    IL_00b1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00bb:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_00c0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ca:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_00cf:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00d4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d9:  ldc.i4.2
    IL_00da:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00df:  dup
    IL_00e0:  ldc.i4.0
    IL_00e1:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_00e6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00eb:  ldtoken    [mscorlib]System.Type
    IL_00f0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fa:  stelem.ref
    IL_00fb:  dup
    IL_00fc:  ldc.i4.1
    IL_00fd:  ldnull
    IL_00fe:  ldtoken    [mscorlib]System.Object
    IL_0103:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0108:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010d:  stelem.ref
    IL_010e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0113:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_0118:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0122:  stelem.ref
    IL_0123:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0128:  ldc.i4.0
    IL_0129:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0133:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0138:  pop
    IL_0139:  ldloc.0
    IL_013a:  newobj     instance void class [System.Core]System.Collections.Generic.HashSet`1<int32>::.ctor()
    IL_013f:  stfld      class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::set
    IL_0144:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0149:  ldnull
    IL_014a:  ldtoken    method bool [System.Core]System.Linq.Enumerable::All<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
    IL_014f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0154:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0159:  ldc.i4.2
    IL_015a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015f:  dup
    IL_0160:  ldc.i4.0
    IL_0161:  ldtoken    [mscorlib]System.Int32
    IL_0166:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016b:  ldc.i4.4
    IL_016c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0171:  dup
    IL_0172:  ldc.i4.0
    IL_0173:  ldc.i4     0x7d0
    IL_0178:  box        [mscorlib]System.Int32
    IL_017d:  ldtoken    [mscorlib]System.Int32
    IL_0182:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0187:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_018c:  stelem.ref
    IL_018d:  dup
    IL_018e:  ldc.i4.1
    IL_018f:  ldc.i4     0x7d4
    IL_0194:  box        [mscorlib]System.Int32
    IL_0199:  ldtoken    [mscorlib]System.Int32
    IL_019e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a8:  stelem.ref
    IL_01a9:  dup
    IL_01aa:  ldc.i4.2
    IL_01ab:  ldc.i4     0x7d8
    IL_01b0:  box        [mscorlib]System.Int32
    IL_01b5:  ldtoken    [mscorlib]System.Int32
    IL_01ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01bf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01c4:  stelem.ref
    IL_01c5:  dup
    IL_01c6:  ldc.i4.3
    IL_01c7:  ldc.i4     0x7dc
    IL_01cc:  box        [mscorlib]System.Int32
    IL_01d1:  ldtoken    [mscorlib]System.Int32
    IL_01d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01db:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01e0:  stelem.ref
    IL_01e1:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01e6:  stelem.ref
    IL_01e7:  dup
    IL_01e8:  ldc.i4.1
    IL_01e9:  ldtoken    method instance bool class [System.Core]System.Collections.Generic.HashSet`1<int32>::Add(!0)
    IL_01ee:  ldtoken    class [System.Core]System.Collections.Generic.HashSet`1<int32>
    IL_01f3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f8:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01fd:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0202:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0207:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020c:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0211:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0216:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_021b:  ldc.i4.2
    IL_021c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0221:  dup
    IL_0222:  ldc.i4.0
    IL_0223:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0228:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022d:  ldtoken    [mscorlib]System.Type
    IL_0232:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0237:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_023c:  stelem.ref
    IL_023d:  dup
    IL_023e:  ldc.i4.1
    IL_023f:  ldloc.0
    IL_0240:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'
    IL_0245:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_024a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_024f:  ldtoken    field class [System.Core]System.Collections.Generic.HashSet`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::set
    IL_0254:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0259:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_025e:  stelem.ref
    IL_025f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0264:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0269:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_026e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0273:  stelem.ref
    IL_0274:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0279:  ldc.i4.0
    IL_027a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_027f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0284:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0289:  pop
    IL_028a:  ldloc.0
    IL_028b:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__33_0'
    IL_0290:  dup
    IL_0291:  brtrue.s   IL_02aa

    IL_0293:  pop
    IL_0294:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0299:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<MethodGroupConstant>b__33_0'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_029f:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_02a4:  dup
    IL_02a5:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__33_0'
    IL_02aa:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::sink
    IL_02af:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_02b4:  ldloc.0
    IL_02b5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'
    IL_02ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02bf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02c4:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass33_0'::sink
    IL_02c9:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02ce:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02d3:  ldc.i4.1
    IL_02d4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02d9:  dup
    IL_02da:  ldc.i4.0
    IL_02db:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02e0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02e5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02ea:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_02ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02f9:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_02fe:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0303:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0308:  ldc.i4.2
    IL_0309:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_030e:  dup
    IL_030f:  ldc.i4.0
    IL_0310:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0315:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_031a:  ldtoken    [mscorlib]System.Type
    IL_031f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0324:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0329:  stelem.ref
    IL_032a:  dup
    IL_032b:  ldc.i4.1
    IL_032c:  ldnull
    IL_032d:  ldtoken    [mscorlib]System.Object
    IL_0332:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0337:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_033c:  stelem.ref
    IL_033d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0342:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0347:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0351:  stelem.ref
    IL_0352:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0357:  ldc.i4.0
    IL_0358:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_035d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0362:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0367:  pop
    IL_0368:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldc.i4.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldtoken    [mscorlib]System.Int32
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldc.i4.1
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  ldtoken    [mscorlib]System.Int32
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  ldtoken    [mscorlib]System.Object
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_003f:  ldtoken    [mscorlib]System.Int32
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0053:  ldc.i4.0
    IL_0054:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0063:  pop
    IL_0064:  ret
  } // end of method ExpressionTrees::MultipleCasts

  .method public hidebysig instance void 
          MultipleDots() cil managed
  {
    // Code size       143 (0x8f)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldc.i4.3
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldtoken    [mscorlib]System.Int32
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.0
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0035:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_003a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0044:  ldc.i4.0
    IL_0045:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_004a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004f:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0054:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0059:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0063:  ldc.i4.0
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_007d:  ldc.i4.0
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0083:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0088:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008d:  pop
    IL_008e:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       548 (0x224)
    .maxstack  12
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0' V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__36_0'
    IL_000d:  dup
    IL_000e:  brtrue.s   IL_0027

    IL_0010:  pop
    IL_0011:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0016:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NestedLambda>b__36_0'(class [mscorlib]System.Func`1<int32>)
    IL_001c:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_0021:  dup
    IL_0022:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__36_0'
    IL_0027:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'::'call'
    IL_002c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0041:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass36_0'::'call'
    IL_0046:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0050:  ldc.i4.1
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0056:  dup
    IL_0057:  ldc.i4.0
    IL_0058:  ldc.i4.s   42
    IL_005a:  box        [mscorlib]System.Int32
    IL_005f:  ldtoken    [mscorlib]System.Int32
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006e:  ldc.i4.0
    IL_006f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0074:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0079:  stelem.ref
    IL_007a:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_007f:  ldc.i4.0
    IL_0080:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0085:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008f:  pop
    IL_0090:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0095:  ldnull
    IL_0096:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_009b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00a0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a5:  ldc.i4.2
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ab:  dup
    IL_00ac:  ldc.i4.0
    IL_00ad:  ldtoken    [mscorlib]System.Int32
    IL_00b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b7:  ldc.i4.2
    IL_00b8:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bd:  dup
    IL_00be:  ldc.i4.0
    IL_00bf:  ldc.i4.s   37
    IL_00c1:  box        [mscorlib]System.Int32
    IL_00c6:  ldtoken    [mscorlib]System.Int32
    IL_00cb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d5:  stelem.ref
    IL_00d6:  dup
    IL_00d7:  ldc.i4.1
    IL_00d8:  ldc.i4.s   42
    IL_00da:  box        [mscorlib]System.Int32
    IL_00df:  ldtoken    [mscorlib]System.Int32
    IL_00e4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00ee:  stelem.ref
    IL_00ef:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f4:  stelem.ref
    IL_00f5:  dup
    IL_00f6:  ldc.i4.1
    IL_00f7:  ldtoken    [mscorlib]System.Int32
    IL_00fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0101:  ldstr      "x"
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010b:  stloc.1
    IL_010c:  ldloc.1
    IL_010d:  ldc.i4.2
    IL_010e:  box        [mscorlib]System.Int32
    IL_0113:  ldtoken    [mscorlib]System.Int32
    IL_0118:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0122:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0127:  ldc.i4.1
    IL_0128:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_012d:  dup
    IL_012e:  ldc.i4.0
    IL_012f:  ldloc.1
    IL_0130:  stelem.ref
    IL_0131:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0136:  stelem.ref
    IL_0137:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_013c:  ldc.i4.0
    IL_013d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0142:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0147:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_014c:  pop
    IL_014d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0152:  ldnull
    IL_0153:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`3<!!0,int32,!!1>)
    IL_0158:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0162:  ldc.i4.2
    IL_0163:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0168:  dup
    IL_0169:  ldc.i4.0
    IL_016a:  ldtoken    [mscorlib]System.Int32
    IL_016f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0174:  ldc.i4.2
    IL_0175:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_017a:  dup
    IL_017b:  ldc.i4.0
    IL_017c:  ldc.i4.s   37
    IL_017e:  box        [mscorlib]System.Int32
    IL_0183:  ldtoken    [mscorlib]System.Int32
    IL_0188:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0192:  stelem.ref
    IL_0193:  dup
    IL_0194:  ldc.i4.1
    IL_0195:  ldc.i4.s   42
    IL_0197:  box        [mscorlib]System.Int32
    IL_019c:  ldtoken    [mscorlib]System.Int32
    IL_01a1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ab:  stelem.ref
    IL_01ac:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01b1:  stelem.ref
    IL_01b2:  dup
    IL_01b3:  ldc.i4.1
    IL_01b4:  ldtoken    [mscorlib]System.Int32
    IL_01b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01be:  ldstr      "x"
    IL_01c3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01c8:  stloc.1
    IL_01c9:  ldtoken    [mscorlib]System.Int32
    IL_01ce:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d3:  ldstr      "i"
    IL_01d8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01dd:  stloc.2
    IL_01de:  ldloc.1
    IL_01df:  ldc.i4.2
    IL_01e0:  box        [mscorlib]System.Int32
    IL_01e5:  ldtoken    [mscorlib]System.Int32
    IL_01ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ef:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_01f9:  ldc.i4.2
    IL_01fa:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ff:  dup
    IL_0200:  ldc.i4.0
    IL_0201:  ldloc.1
    IL_0202:  stelem.ref
    IL_0203:  dup
    IL_0204:  ldc.i4.1
    IL_0205:  ldloc.2
    IL_0206:  stelem.ref
    IL_0207:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_020c:  stelem.ref
    IL_020d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0212:  ldc.i4.0
    IL_0213:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0218:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_021d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0222:  pop
    IL_0223:  ret
  } // end of method ExpressionTrees::NestedLambda

  .method public hidebysig instance void 
          CurriedLambda() cil managed
  {
    // Code size       134 (0x86)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "a"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldtoken    [mscorlib]System.Int32
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldstr      "b"
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002f:  stloc.1
    IL_0030:  ldtoken    [mscorlib]System.Int32
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  ldstr      "c"
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0044:  stloc.2
    IL_0045:  ldloc.0
    IL_0046:  ldloc.1
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.2
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.1
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  dup
    IL_0059:  ldc.i4.0
    IL_005a:  ldloc.2
    IL_005b:  stelem.ref
    IL_005c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0061:  ldc.i4.1
    IL_0062:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0067:  dup
    IL_0068:  ldc.i4.0
    IL_0069:  ldloc.1
    IL_006a:  stelem.ref
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  ldc.i4.1
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0076:  dup
    IL_0077:  ldc.i4.0
    IL_0078:  ldloc.0
    IL_0079:  stelem.ref
    IL_007a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0084:  pop
    IL_0085:  ret
  } // end of method ExpressionTrees::CurriedLambda

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   42
    IL_0004:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Buzz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   42
    IL_0004:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::Buzz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<string,bool> a) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldstr      "42"
    IL_0007:  callvirt   instance !1 class [mscorlib]System.Func`2<string,bool>::Invoke(!0)
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool> a) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ExpressionTrees::Fizz

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       1125 (0x465)
    .maxstack  14
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0016:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_001b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0020:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0025:  ldc.i4.1
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002b:  dup
    IL_002c:  ldc.i4.0
    IL_002d:  ldtoken    [mscorlib]System.String
    IL_0032:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0037:  ldstr      "x"
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0041:  stloc.0
    IL_0042:  ldloc.0
    IL_0043:  ldstr      "a"
    IL_0048:  ldtoken    [mscorlib]System.String
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_005c:  ldc.i4.1
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0062:  dup
    IL_0063:  ldc.i4.0
    IL_0064:  ldloc.0
    IL_0065:  stelem.ref
    IL_0066:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006b:  stelem.ref
    IL_006c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0071:  ldc.i4.0
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0081:  pop
    IL_0082:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0087:  ldarg.0
    IL_0088:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_008d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0092:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0097:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_009c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00a1:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a6:  ldc.i4.1
    IL_00a7:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ac:  dup
    IL_00ad:  ldc.i4.0
    IL_00ae:  ldtoken    [mscorlib]System.String
    IL_00b3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b8:  ldstr      "x"
    IL_00bd:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00c2:  stloc.0
    IL_00c3:  ldloc.0
    IL_00c4:  ldstr      "a"
    IL_00c9:  ldtoken    [mscorlib]System.String
    IL_00ce:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00dd:  ldc.i4.1
    IL_00de:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e3:  dup
    IL_00e4:  ldc.i4.0
    IL_00e5:  ldloc.0
    IL_00e6:  stelem.ref
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ec:  stelem.ref
    IL_00ed:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f2:  ldc.i4.0
    IL_00f3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00fd:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0102:  pop
    IL_0103:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0108:  ldarg.0
    IL_0109:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_010e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0113:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0118:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_011d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0122:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0127:  ldc.i4.1
    IL_0128:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_012d:  dup
    IL_012e:  ldc.i4.0
    IL_012f:  ldtoken    [mscorlib]System.Action
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  ldstr      "x"
    IL_013e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0143:  stloc.0
    IL_0144:  ldloc.0
    IL_0145:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_014a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_014f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0154:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0159:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0163:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0168:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_016d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0172:  ldc.i4.2
    IL_0173:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0178:  dup
    IL_0179:  ldc.i4.0
    IL_017a:  ldtoken    [mscorlib]System.Action
    IL_017f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0184:  ldtoken    [mscorlib]System.Type
    IL_0189:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0193:  stelem.ref
    IL_0194:  dup
    IL_0195:  ldc.i4.1
    IL_0196:  ldarg.0
    IL_0197:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_019c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a6:  stelem.ref
    IL_01a7:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ac:  ldtoken    [mscorlib]System.Action
    IL_01b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b6:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_01bb:  ldc.i4.0
    IL_01bc:  ldtoken    method bool [mscorlib]System.Delegate::op_Equality(class [mscorlib]System.Delegate,
                                                                            class [mscorlib]System.Delegate)
    IL_01c1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01c6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01cb:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_01d0:  ldc.i4.1
    IL_01d1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01d6:  dup
    IL_01d7:  ldc.i4.0
    IL_01d8:  ldloc.0
    IL_01d9:  stelem.ref
    IL_01da:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01df:  stelem.ref
    IL_01e0:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01e5:  ldc.i4.0
    IL_01e6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01eb:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01f0:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01f5:  pop
    IL_01f6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_01fb:  ldarg.0
    IL_01fc:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0201:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0206:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020b:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>)
    IL_0210:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0215:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_021a:  ldc.i4.1
    IL_021b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0220:  dup
    IL_0221:  ldc.i4.0
    IL_0222:  ldtoken    [mscorlib]System.Action
    IL_0227:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022c:  ldstr      "x"
    IL_0231:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0236:  stloc.0
    IL_0237:  ldloc.0
    IL_0238:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::NestedLambda2()
    IL_023d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0242:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0247:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_024c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0251:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0256:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_025b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0260:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0265:  ldc.i4.2
    IL_0266:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_026b:  dup
    IL_026c:  ldc.i4.0
    IL_026d:  ldtoken    [mscorlib]System.Action
    IL_0272:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0277:  ldtoken    [mscorlib]System.Type
    IL_027c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0281:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0286:  stelem.ref
    IL_0287:  dup
    IL_0288:  ldc.i4.1
    IL_0289:  ldarg.0
    IL_028a:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_028f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0294:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0299:  stelem.ref
    IL_029a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_029f:  ldtoken    [mscorlib]System.Action
    IL_02a4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a9:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02ae:  ldc.i4.0
    IL_02af:  ldtoken    method bool [mscorlib]System.Delegate::op_Inequality(class [mscorlib]System.Delegate,
                                                                              class [mscorlib]System.Delegate)
    IL_02b4:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02b9:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02be:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               bool,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_02c3:  ldc.i4.1
    IL_02c4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02c9:  dup
    IL_02ca:  ldc.i4.0
    IL_02cb:  ldloc.0
    IL_02cc:  stelem.ref
    IL_02cd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class [mscorlib]System.Action,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02d2:  stelem.ref
    IL_02d3:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_02d8:  ldc.i4.0
    IL_02d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02e3:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_02e8:  pop
    IL_02e9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_02ee:  ldarg.0
    IL_02ef:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_02f4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02fe:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0303:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0308:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_030d:  ldc.i4.1
    IL_030e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0313:  dup
    IL_0314:  ldc.i4.0
    IL_0315:  ldtoken    [mscorlib]System.Int32
    IL_031a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_031f:  ldstr      "x"
    IL_0324:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0329:  stloc.0
    IL_032a:  ldloc.0
    IL_032b:  ldc.i4.s   37
    IL_032d:  box        [mscorlib]System.Int32
    IL_0332:  ldtoken    [mscorlib]System.Int32
    IL_0337:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0341:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0346:  ldc.i4.1
    IL_0347:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_034c:  dup
    IL_034d:  ldc.i4.0
    IL_034e:  ldloc.0
    IL_034f:  stelem.ref
    IL_0350:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0355:  stelem.ref
    IL_0356:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_035b:  ldc.i4.0
    IL_035c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0361:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0366:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_036b:  pop
    IL_036c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0371:  ldarg.0
    IL_0372:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0377:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_037c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0381:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0386:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_038b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0390:  ldc.i4.1
    IL_0391:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0396:  dup
    IL_0397:  ldc.i4.0
    IL_0398:  ldtoken    [mscorlib]System.Int32
    IL_039d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03a2:  ldstr      "x"
    IL_03a7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03ac:  stloc.0
    IL_03ad:  ldc.i4.1
    IL_03ae:  box        [mscorlib]System.Boolean
    IL_03b3:  ldtoken    [mscorlib]System.Boolean
    IL_03b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03bd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03c2:  ldc.i4.1
    IL_03c3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03c8:  dup
    IL_03c9:  ldc.i4.0
    IL_03ca:  ldloc.0
    IL_03cb:  stelem.ref
    IL_03cc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03d1:  stelem.ref
    IL_03d2:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_03d7:  ldc.i4.0
    IL_03d8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03dd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03e2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_03e7:  pop
    IL_03e8:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_03ed:  ldarg.0
    IL_03ee:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_03f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03f8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_03fd:  ldtoken    method instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0402:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0407:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_040c:  ldc.i4.1
    IL_040d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0412:  dup
    IL_0413:  ldc.i4.0
    IL_0414:  ldtoken    [mscorlib]System.Int32
    IL_0419:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_041e:  ldstr      "x"
    IL_0423:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0428:  stloc.0
    IL_0429:  ldc.i4.1
    IL_042a:  box        [mscorlib]System.Boolean
    IL_042f:  ldtoken    [mscorlib]System.Boolean
    IL_0434:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0439:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_043e:  ldc.i4.1
    IL_043f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0444:  dup
    IL_0445:  ldc.i4.0
    IL_0446:  ldloc.0
    IL_0447:  stelem.ref
    IL_0448:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_044d:  stelem.ref
    IL_044e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0453:  ldc.i4.0
    IL_0454:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0459:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_045e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0463:  pop
    IL_0464:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       291 (0x123)
    .maxstack  12
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [System.Core]System.Linq.Enumerable::SequenceEqual<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldtoken    [mscorlib]System.Double
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  ldc.i4.3
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  dup
    IL_002f:  ldc.i4.0
    IL_0030:  ldc.r8     1.
    IL_0039:  box        [mscorlib]System.Double
    IL_003e:  ldtoken    [mscorlib]System.Double
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  stelem.ref
    IL_004e:  dup
    IL_004f:  ldc.i4.1
    IL_0050:  ldc.r8     2.0099999999999998
    IL_0059:  box        [mscorlib]System.Double
    IL_005e:  ldtoken    [mscorlib]System.Double
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  stelem.ref
    IL_006e:  dup
    IL_006f:  ldc.i4.2
    IL_0070:  ldc.r8     3.5
    IL_0079:  box        [mscorlib]System.Double
    IL_007e:  ldtoken    [mscorlib]System.Double
    IL_0083:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008d:  stelem.ref
    IL_008e:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0093:  stelem.ref
    IL_0094:  dup
    IL_0095:  ldc.i4.1
    IL_0096:  ldtoken    [mscorlib]System.Double
    IL_009b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a0:  ldc.i4.3
    IL_00a1:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a6:  dup
    IL_00a7:  ldc.i4.0
    IL_00a8:  ldc.r8     1.
    IL_00b1:  box        [mscorlib]System.Double
    IL_00b6:  ldtoken    [mscorlib]System.Double
    IL_00bb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c5:  stelem.ref
    IL_00c6:  dup
    IL_00c7:  ldc.i4.1
    IL_00c8:  ldc.r8     2.0099999999999998
    IL_00d1:  box        [mscorlib]System.Double
    IL_00d6:  ldtoken    [mscorlib]System.Double
    IL_00db:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e5:  stelem.ref
    IL_00e6:  dup
    IL_00e7:  ldc.i4.2
    IL_00e8:  ldc.r8     3.5
    IL_00f1:  box        [mscorlib]System.Double
    IL_00f6:  ldtoken    [mscorlib]System.Double
    IL_00fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0100:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0105:  stelem.ref
    IL_0106:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_010b:  stelem.ref
    IL_010c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0111:  ldc.i4.0
    IL_0112:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0117:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0121:  pop
    IL_0122:  ret
  } // end of method ExpressionTrees::NewArrayAndExtensionMethod

  .method public hidebysig instance void 
          NewMultiDimArray() cil managed
  {
    // Code size       139 (0x8b)
    .maxstack  7
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  dup
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.3
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.4
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_004b:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_005f:  ldc.i4.1
    IL_0060:  box        [mscorlib]System.Int32
    IL_0065:  ldtoken    [mscorlib]System.Int32
    IL_006a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0074:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0079:  ldc.i4.0
    IL_007a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0084:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0089:  pop
    IL_008a:  ret
  } // end of method ExpressionTrees::NewMultiDimArray

  .method public hidebysig instance void 
          NewObject() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Object
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldtoken    [mscorlib]System.Object
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0029:  ldc.i4.0
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0034:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0039:  pop
    IL_003a:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       271 (0x10f)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::x
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.3
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::y
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s   42
    IL_0018:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::z
    IL_001d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0022:  ldloc.0
    IL_0023:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldtoken    field uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::z
    IL_0037:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0041:  ldtoken    [mscorlib]System.Int32
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0050:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0055:  ldc.i4.0
    IL_0056:  box        [mscorlib]System.Int32
    IL_005b:  ldtoken    [mscorlib]System.Int32
    IL_0060:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_006f:  ldc.i4.0
    IL_0070:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0075:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007f:  pop
    IL_0080:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0085:  ldloc.0
    IL_0086:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0095:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::y
    IL_009a:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_009f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00a9:  ldc.i4.0
    IL_00aa:  box        [mscorlib]System.Int32
    IL_00af:  ldtoken    [mscorlib]System.Int32
    IL_00b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00be:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00c3:  ldc.i4.0
    IL_00c4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00c9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ce:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00d3:  pop
    IL_00d4:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_00d9:  ldloc.0
    IL_00da:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'
    IL_00df:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e9:  ldtoken    field bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass46_0'::x
    IL_00ee:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f3:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00fd:  ldc.i4.0
    IL_00fe:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0103:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0108:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_010d:  pop
    IL_010e:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       291 (0x123)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_000d:  dup
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0014:  nop
    IL_0015:  dup
    IL_0016:  ldc.i4.0
    IL_0017:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_001c:  nop
    IL_001d:  stfld      class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_0022:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0027:  ldtoken    [System.Xml]System.Xml.XmlReaderSettings
    IL_002c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0036:  ldc.i4.2
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_003c:  dup
    IL_003d:  ldc.i4.0
    IL_003e:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0043:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0048:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_004d:  ldloc.0
    IL_004e:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_0062:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0067:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_006c:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CloseInput()
    IL_0071:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0076:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0080:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0085:  stelem.ref
    IL_0086:  dup
    IL_0087:  ldc.i4.1
    IL_0088:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_008d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0092:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0097:  ldloc.0
    IL_0098:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a7:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_00ac:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00b6:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CheckCharacters()
    IL_00bb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00c0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00cf:  stelem.ref
    IL_00d0:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00d5:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00da:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00df:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e4:  ldc.i4.1
    IL_00e5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ea:  dup
    IL_00eb:  ldc.i4.0
    IL_00ec:  ldloc.0
    IL_00ed:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fc:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_0101:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0106:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_010b:  stelem.ref
    IL_010c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0111:  ldc.i4.0
    IL_0112:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0117:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0121:  pop
    IL_0122:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       174 (0xae)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "n"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldtoken    [mscorlib]System.String
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldstr      "s"
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.0
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004c:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0060:  ldc.i4.2
    IL_0061:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0066:  dup
    IL_0067:  ldc.i4.0
    IL_0068:  ldloc.0
    IL_0069:  stelem.ref
    IL_006a:  dup
    IL_006b:  ldc.i4.1
    IL_006c:  ldloc.1
    IL_006d:  stelem.ref
    IL_006e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0073:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0078:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0087:  ldnull
    IL_0088:  ldtoken    [mscorlib]System.Object
    IL_008d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0092:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0097:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_009c:  ldc.i4.0
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a7:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ac:  pop
    IL_00ad:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       166 (0xa6)
    .maxstack  9
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0039:  stelem.ref
    IL_003a:  dup
    IL_003b:  ldc.i4.1
    IL_003c:  ldc.i4.1
    IL_003d:  box        [mscorlib]System.Boolean
    IL_0042:  ldtoken    [mscorlib]System.Boolean
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  ldc.i4.0
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0061:  stelem.ref
    IL_0062:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0067:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_006c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0071:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0076:  ldc.i4.1
    IL_0077:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007c:  dup
    IL_007d:  ldc.i4.0
    IL_007e:  ldnull
    IL_007f:  ldtoken    [mscorlib]System.Object
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008e:  stelem.ref
    IL_008f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0094:  ldc.i4.0
    IL_0095:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00a4:  pop
    IL_00a5:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       348 (0x15c)
    .maxstack  22
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method !!0 [System.Core]System.Linq.Enumerable::Single<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.1
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<string,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.2
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  dup
    IL_0035:  ldc.i4.0
    IL_0036:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  ldc.i4.1
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  dup
    IL_0047:  ldc.i4.0
    IL_0048:  ldtoken    method instance void class '<>f__AnonymousType1`2'<string,string>::.ctor(!0,
                                                                                                  !1)
    IL_004d:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005c:  ldc.i4.2
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0062:  dup
    IL_0063:  ldc.i4.0
    IL_0064:  ldstr      "a"
    IL_0069:  ldtoken    [mscorlib]System.String
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  stelem.ref
    IL_0079:  dup
    IL_007a:  ldc.i4.1
    IL_007b:  ldstr      "b"
    IL_0080:  ldtoken    [mscorlib]System.String
    IL_0085:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008f:  stelem.ref
    IL_0090:  ldc.i4.2
    IL_0091:  newarr     [mscorlib]System.Reflection.MemberInfo
    IL_0096:  dup
    IL_0097:  ldc.i4.0
    IL_0098:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_009d:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00a2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ac:  stelem.ref
    IL_00ad:  dup
    IL_00ae:  ldc.i4.1
    IL_00af:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_00b4:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00b9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00be:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c3:  stelem.ref
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00c9:  stelem.ref
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00cf:  stelem.ref
    IL_00d0:  dup
    IL_00d1:  ldc.i4.1
    IL_00d2:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  ldstr      "o"
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e6:  stloc.0
    IL_00e7:  ldloc.0
    IL_00e8:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00ed:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00f2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0101:  ldloc.0
    IL_0102:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_0107:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_010c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0111:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0116:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011b:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0120:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0125:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_012a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_012f:  ldc.i4.1
    IL_0130:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0135:  dup
    IL_0136:  ldc.i4.0
    IL_0137:  ldloc.0
    IL_0138:  stelem.ref
    IL_0139:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<string,string>,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_013e:  stelem.ref
    IL_013f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0144:  stelem.ref
    IL_0145:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_014a:  ldc.i4.0
    IL_014b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0150:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0155:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_015a:  pop
    IL_015b:  ret
  } // end of method ExpressionTrees::QuotedWithAnonymous

  .method public hidebysig instance void 
          StaticCall() cil managed
  {
    // Code size       129 (0x81)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.3
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  ldtoken    [mscorlib]System.Object
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  stelem.ref
    IL_0043:  dup
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    [mscorlib]System.Object
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006f:  ldc.i4.0
    IL_0070:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0075:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007f:  pop
    IL_0080:  ret
  } // end of method ExpressionTrees::StaticCall

  .method public hidebysig instance void 
          ThisCall() cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0016:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_001b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0020:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0025:  ldc.i4.1
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002b:  dup
    IL_002c:  ldc.i4.0
    IL_002d:  ldc.i4.3
    IL_002e:  box        [mscorlib]System.Int32
    IL_0033:  ldtoken    [mscorlib]System.Int32
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  ldtoken    [mscorlib]System.Object
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0051:  stelem.ref
    IL_0052:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0057:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_005c:  ldc.i4.0
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0062:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0067:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006c:  pop
    IL_006d:  ret
  } // end of method ExpressionTrees::ThisCall

  .method public hidebysig instance void 
          ThisExplicit() cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldarg.0
    IL_001f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  stelem.ref
    IL_002f:  dup
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.3
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  ldtoken    [mscorlib]System.Object
    IL_004b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0050:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0055:  stelem.ref
    IL_0056:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006b:  pop
    IL_006c:  ret
  } // end of method ExpressionTrees::ThisExplicit

  .method public hidebysig instance void 
          TypedConstant() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  7
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Type
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  dup
    IL_0017:  ldc.i4.0
    IL_0018:  ldtoken    [mscorlib]System.Int32
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  ldtoken    [mscorlib]System.Type
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  stelem.ref
    IL_0032:  dup
    IL_0033:  ldc.i4.1
    IL_0034:  ldtoken    [mscorlib]System.String
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  ldtoken    [mscorlib]System.Type
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  stelem.ref
    IL_004e:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0053:  ldc.i4.0
    IL_0054:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0063:  pop
    IL_0064:  ret
  } // end of method ExpressionTrees::TypedConstant

  .method public hidebysig instance void 
          StaticCallImplicitCast() cil managed
  {
    // Code size       129 (0x81)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.3
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  ldtoken    [mscorlib]System.Object
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  stelem.ref
    IL_0043:  dup
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    [mscorlib]System.Object
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006f:  ldc.i4.0
    IL_0070:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0075:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007f:  pop
    IL_0080:  ret
  } // end of method ExpressionTrees::StaticCallImplicitCast

  .method public hidebysig instance void 
          StaticMembers() cil managed
  {
    // Code size       217 (0xd9)
    .maxstack  10
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_001b:  ldnull
    IL_001c:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0021:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0026:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0030:  ldnull
    IL_0031:  ldtoken    method valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromMilliseconds(float64)
    IL_0036:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0040:  ldc.i4.1
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  dup
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.r8     10.000999999999999
    IL_0051:  box        [mscorlib]System.Double
    IL_0056:  ldtoken    [mscorlib]System.Double
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  stelem.ref
    IL_0066:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006b:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::op_Addition(valuetype [mscorlib]System.DateTime,
                                                                                                           valuetype [mscorlib]System.TimeSpan)
    IL_0070:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0075:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_007f:  ldc.i4.0
    IL_0080:  ldtoken    method bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                               valuetype [mscorlib]System.DateTime)
    IL_0085:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  bool,
                                                                                                                                                  class [mscorlib]System.Reflection.MethodInfo)
    IL_0094:  ldtoken    method instance string [mscorlib]System.Boolean::ToString()
    IL_0099:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a3:  ldc.i4.0
    IL_00a4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ae:  ldstr      "False"
    IL_00b3:  ldtoken    [mscorlib]System.String
    IL_00b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00c7:  ldc.i4.0
    IL_00c8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d2:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00d7:  pop
    IL_00d8:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       407 (0x197)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_0019:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_0047:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0056:  ldloc.0
    IL_0057:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  ldtoken    field string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_006b:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0075:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0089:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_008e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0093:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0098:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_009d:  ldc.i4.2
    IL_009e:  box        [mscorlib]System.Int32
    IL_00a3:  ldtoken    [mscorlib]System.Int32
    IL_00a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00b7:  ldc.i4.0
    IL_00b8:  box        [mscorlib]System.Boolean
    IL_00bd:  ldtoken    [mscorlib]System.Boolean
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldc.i4.1
    IL_00cd:  box        [mscorlib]System.Boolean
    IL_00d2:  ldtoken    [mscorlib]System.Boolean
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e1:  ldc.i4.1
    IL_00e2:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00e7:  box        [mscorlib]System.Decimal
    IL_00ec:  ldtoken    [mscorlib]System.Decimal
    IL_00f1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fb:  ldloc.0
    IL_00fc:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'
    IL_0101:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010b:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass57_0'::i
    IL_0110:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0115:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_011a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_011f:  ldtoken    [mscorlib]System.Decimal
    IL_0124:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0129:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_012e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0133:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0138:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_013d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0142:  ldc.i4.0
    IL_0143:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0148:  box        [mscorlib]System.Decimal
    IL_014d:  ldtoken    [mscorlib]System.Decimal
    IL_0152:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  ldc.i4.0
    IL_0162:  box        [mscorlib]System.Boolean
    IL_0167:  ldtoken    [mscorlib]System.Boolean
    IL_016c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0171:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0176:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_017b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0180:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0185:  ldc.i4.0
    IL_0186:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0190:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0195:  pop
    IL_0196:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       119 (0x77)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_001a:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_001f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0029:  ldtoken    [mscorlib]System.Double
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0038:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0047:  ldtoken    method instance !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_004c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0075:  pop
    IL_0076:  ret
  } // end of method ExpressionTrees::GenericClassInstance

  .method public hidebysig instance void 
          GenericClassStatic() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    field !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001b:  ldtoken    [mscorlib]System.Double
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_002a:  ldnull
    IL_002b:  ldtoken    method !0 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_0030:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<float64>
    IL_0035:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0044:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0049:  ldc.i4.0
    IL_004a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0054:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<float64>(object,
                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0059:  pop
    IL_005a:  ret
  } // end of method ExpressionTrees::GenericClassStatic

  .method public hidebysig instance void 
          InvokeGenericMethod() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001b:  ldc.i4.0
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0021:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0026:  ldc.i4.0
    IL_0027:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0031:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool>(object,
                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0036:  pop
    IL_0037:  ret
  } // end of method ExpressionTrees::InvokeGenericMethod

  .method private hidebysig static void  Test<T>(!!T delegateExpression,
                                                 class [System.Core]System.Linq.Expressions.Expression`1<!!T> expressionTree) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method ExpressionTrees::Test

  .method public hidebysig static void  ArrayIndexer() cil managed
  {
    // Code size       609 (0x261)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_0'(int32[])
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_0'
    IL_0020:  ldtoken    int32[]
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "array"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.0
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0050:  ldc.i4.1
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0056:  dup
    IL_0057:  ldc.i4.0
    IL_0058:  ldloc.0
    IL_0059:  stelem.ref
    IL_005a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0064:  nop
    IL_0065:  ldsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_1'
    IL_006a:  dup
    IL_006b:  brtrue.s   IL_0084

    IL_006d:  pop
    IL_006e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0073:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_1'(int32[],
                                                                                                                                     int32)
    IL_0079:  newobj     instance void class [mscorlib]System.Func`3<int32[],int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_007e:  dup
    IL_007f:  stsfld     class [mscorlib]System.Func`3<int32[],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_1'
    IL_0084:  ldtoken    int32[]
    IL_0089:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008e:  ldstr      "array"
    IL_0093:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0098:  stloc.0
    IL_0099:  ldtoken    [mscorlib]System.Int32
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  ldstr      "index"
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ad:  stloc.1
    IL_00ae:  ldloc.0
    IL_00af:  ldloc.1
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00b5:  ldc.i4.2
    IL_00b6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00bb:  dup
    IL_00bc:  ldc.i4.0
    IL_00bd:  ldloc.0
    IL_00be:  stelem.ref
    IL_00bf:  dup
    IL_00c0:  ldc.i4.1
    IL_00c1:  ldloc.1
    IL_00c2:  stelem.ref
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[],int32,int32>>(!!0,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00cd:  nop
    IL_00ce:  ldsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_2'
    IL_00d3:  dup
    IL_00d4:  brtrue.s   IL_00ed

    IL_00d6:  pop
    IL_00d7:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00dc:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_2'(int32[0...,0...])
    IL_00e2:  newobj     instance void class [mscorlib]System.Func`2<int32[0...,0...],int32>::.ctor(object,
                                                                                                    native int)
    IL_00e7:  dup
    IL_00e8:  stsfld     class [mscorlib]System.Func`2<int32[0...,0...],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_2'
    IL_00ed:  ldtoken    int32[0...,0...]
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  ldstr      "array"
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0101:  stloc.1
    IL_0102:  ldloc.1
    IL_0103:  ldc.i4.2
    IL_0104:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0109:  dup
    IL_010a:  ldc.i4.0
    IL_010b:  ldc.i4.0
    IL_010c:  box        [mscorlib]System.Int32
    IL_0111:  ldtoken    [mscorlib]System.Int32
    IL_0116:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0120:  stelem.ref
    IL_0121:  dup
    IL_0122:  ldc.i4.1
    IL_0123:  ldc.i4.5
    IL_0124:  box        [mscorlib]System.Int32
    IL_0129:  ldtoken    [mscorlib]System.Int32
    IL_012e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0133:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0138:  stelem.ref
    IL_0139:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_013e:  ldc.i4.1
    IL_013f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0144:  dup
    IL_0145:  ldc.i4.0
    IL_0146:  ldloc.1
    IL_0147:  stelem.ref
    IL_0148:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_014d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[0...,0...],int32>>(!!0,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0152:  nop
    IL_0153:  ldsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_3'
    IL_0158:  dup
    IL_0159:  brtrue.s   IL_0172

    IL_015b:  pop
    IL_015c:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0161:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_3'(int32[0...,0...],
                                                                                                                                     int32)
    IL_0167:  newobj     instance void class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>::.ctor(object,
                                                                                                          native int)
    IL_016c:  dup
    IL_016d:  stsfld     class [mscorlib]System.Func`3<int32[0...,0...],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_3'
    IL_0172:  ldtoken    int32[0...,0...]
    IL_0177:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017c:  ldstr      "array"
    IL_0181:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0186:  stloc.1
    IL_0187:  ldtoken    [mscorlib]System.Int32
    IL_018c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0191:  ldstr      "index"
    IL_0196:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_019b:  stloc.0
    IL_019c:  ldloc.1
    IL_019d:  ldc.i4.2
    IL_019e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01a3:  dup
    IL_01a4:  ldc.i4.0
    IL_01a5:  ldloc.0
    IL_01a6:  stelem.ref
    IL_01a7:  dup
    IL_01a8:  ldc.i4.1
    IL_01a9:  ldc.i4.7
    IL_01aa:  box        [mscorlib]System.Int32
    IL_01af:  ldtoken    [mscorlib]System.Int32
    IL_01b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01be:  stelem.ref
    IL_01bf:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01c4:  ldc.i4.2
    IL_01c5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ca:  dup
    IL_01cb:  ldc.i4.0
    IL_01cc:  ldloc.1
    IL_01cd:  stelem.ref
    IL_01ce:  dup
    IL_01cf:  ldc.i4.1
    IL_01d0:  ldloc.0
    IL_01d1:  stelem.ref
    IL_01d2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01d7:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[0...,0...],int32,int32>>(!!0,
                                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01dc:  nop
    IL_01dd:  ldsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_4'
    IL_01e2:  dup
    IL_01e3:  brtrue.s   IL_01fc

    IL_01e5:  pop
    IL_01e6:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01eb:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayIndexer>b__62_4'(int32[][],
                                                                                                                                     int32)
    IL_01f1:  newobj     instance void class [mscorlib]System.Func`3<int32[][],int32,int32>::.ctor(object,
                                                                                                   native int)
    IL_01f6:  dup
    IL_01f7:  stsfld     class [mscorlib]System.Func`3<int32[][],int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__62_4'
    IL_01fc:  ldtoken    int32[][]
    IL_0201:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0206:  ldstr      "array"
    IL_020b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0210:  stloc.0
    IL_0211:  ldtoken    [mscorlib]System.Int32
    IL_0216:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021b:  ldstr      "index"
    IL_0220:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0225:  stloc.1
    IL_0226:  ldloc.0
    IL_0227:  ldloc.1
    IL_0228:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_022d:  ldc.i4.7
    IL_022e:  box        [mscorlib]System.Int32
    IL_0233:  ldtoken    [mscorlib]System.Int32
    IL_0238:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_023d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0242:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0247:  ldc.i4.2
    IL_0248:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_024d:  dup
    IL_024e:  ldc.i4.0
    IL_024f:  ldloc.0
    IL_0250:  stelem.ref
    IL_0251:  dup
    IL_0252:  ldc.i4.1
    IL_0253:  ldloc.1
    IL_0254:  stelem.ref
    IL_0255:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32[][],int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_025a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32[][],int32,int32>>(!!0,
                                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_025f:  nop
    IL_0260:  ret
  } // end of method ExpressionTrees::ArrayIndexer

  .method public hidebysig static void  ArrayLength() cil managed
  {
    // Code size       165 (0xa5)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayLength>b__63_0'(int32[])
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32[],int32>::.ctor(object,
                                                                                           native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<int32[],int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_0'
    IL_0020:  ldtoken    int32[]
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "array"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_003b:  ldc.i4.1
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  dup
    IL_0042:  ldc.i4.0
    IL_0043:  ldloc.0
    IL_0044:  stelem.ref
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32[],int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32[],int32>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004f:  nop
    IL_0050:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_1'
    IL_0055:  dup
    IL_0056:  brtrue.s   IL_006f

    IL_0058:  pop
    IL_0059:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_005e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayLength>b__63_1'()
    IL_0064:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_0069:  dup
    IL_006a:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__63_1'
    IL_006f:  ldnull
    IL_0070:  ldtoken    [mscorlib]System.Array
    IL_0075:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007f:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0084:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0089:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0093:  ldc.i4.0
    IL_0094:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0099:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00a3:  nop
    IL_00a4:  ret
  } // end of method ExpressionTrees::ArrayLength

  .method public hidebysig static void  NewObj() cil managed
  {
    // Code size       553 (0x229)
    .maxstack  7
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_0'
    IL_0020:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_003f:  nop
    IL_0040:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_1'
    IL_0045:  dup
    IL_0046:  brtrue.s   IL_005f

    IL_0048:  pop
    IL_0049:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_004e:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_1'()
    IL_0054:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_0059:  dup
    IL_005a:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_1'
    IL_005f:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor::.ctor(int32)
    IL_0064:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0069:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_006e:  ldc.i4.1
    IL_006f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0074:  dup
    IL_0075:  ldc.i4.0
    IL_0076:  ldc.i4.5
    IL_0077:  box        [mscorlib]System.Int32
    IL_007c:  ldtoken    [mscorlib]System.Int32
    IL_0081:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0086:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008b:  stelem.ref
    IL_008c:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0091:  ldc.i4.0
    IL_0092:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0097:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00a1:  nop
    IL_00a2:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_2'
    IL_00a7:  dup
    IL_00a8:  brtrue.s   IL_00c1

    IL_00aa:  pop
    IL_00ab:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00b0:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_2'()
    IL_00b6:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_00bb:  dup
    IL_00bc:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_2'
    IL_00c1:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors
    IL_00c6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cb:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_00d0:  ldc.i4.0
    IL_00d1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00db:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00e0:  nop
    IL_00e1:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_3'
    IL_00e6:  dup
    IL_00e7:  brtrue.s   IL_0100

    IL_00e9:  pop
    IL_00ea:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00ef:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_3'()
    IL_00f5:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_00fa:  dup
    IL_00fb:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_3'
    IL_0100:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors::.ctor(int32)
    IL_0105:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_010a:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_010f:  ldc.i4.1
    IL_0110:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0115:  dup
    IL_0116:  ldc.i4.0
    IL_0117:  ldc.i4.5
    IL_0118:  box        [mscorlib]System.Int32
    IL_011d:  ldtoken    [mscorlib]System.Int32
    IL_0122:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0127:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_012c:  stelem.ref
    IL_012d:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0132:  ldc.i4.0
    IL_0133:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0138:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_013d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0142:  nop
    IL_0143:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_4'
    IL_0148:  dup
    IL_0149:  brtrue.s   IL_0162

    IL_014b:  pop
    IL_014c:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0151:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_4'()
    IL_0157:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_015c:  dup
    IL_015d:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_4'
    IL_0162:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<int32>
    IL_0167:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016c:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0171:  ldc.i4.0
    IL_0172:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0177:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_017c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0181:  nop
    IL_0182:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_5'
    IL_0187:  dup
    IL_0188:  brtrue.s   IL_01a1

    IL_018a:  pop
    IL_018b:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0190:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_5'()
    IL_0196:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_019b:  dup
    IL_019c:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_5'
    IL_01a1:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<int32>
    IL_01a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ab:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_01b0:  ldc.i4.0
    IL_01b1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01b6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01bb:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01c0:  nop
    IL_01c1:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_6'
    IL_01c6:  dup
    IL_01c7:  brtrue.s   IL_01e0

    IL_01c9:  pop
    IL_01ca:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01cf:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<NewObj>b__64_6'()
    IL_01d5:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_01da:  dup
    IL_01db:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__64_6'
    IL_01e0:  ldtoken    method instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>::.ctor(int32)
    IL_01e5:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<int32>
    IL_01ea:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ef:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01f4:  ldc.i4.1
    IL_01f5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01fa:  dup
    IL_01fb:  ldc.i4.0
    IL_01fc:  ldc.i4.5
    IL_01fd:  box        [mscorlib]System.Int32
    IL_0202:  ldtoken    [mscorlib]System.Int32
    IL_0207:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0211:  stelem.ref
    IL_0212:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>)
    IL_0217:  ldc.i4.0
    IL_0218:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_021d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0222:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0227:  nop
    IL_0228:  ret
  } // end of method ExpressionTrees::NewObj

  .method public hidebysig static void  TypeOfExpr() cil managed
  {
    // Code size       367 (0x16f)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldtoken    [mscorlib]System.Type
    IL_002f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0039:  ldc.i4.0
    IL_003a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0049:  nop
    IL_004a:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_1'
    IL_004f:  dup
    IL_0050:  brtrue.s   IL_0069

    IL_0052:  pop
    IL_0053:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0058:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_1'()
    IL_005e:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_0063:  dup
    IL_0064:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_1'
    IL_0069:  ldtoken    [mscorlib]System.Object
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  ldtoken    [mscorlib]System.Type
    IL_0078:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0082:  ldc.i4.0
    IL_0083:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0088:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0092:  nop
    IL_0093:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_2'
    IL_0098:  dup
    IL_0099:  brtrue.s   IL_00b2

    IL_009b:  pop
    IL_009c:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00a1:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_2'()
    IL_00a7:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00ac:  dup
    IL_00ad:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_2'
    IL_00b2:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_00b7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bc:  ldtoken    [mscorlib]System.Type
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cb:  ldc.i4.0
    IL_00cc:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d6:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00db:  nop
    IL_00dc:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_3'
    IL_00e1:  dup
    IL_00e2:  brtrue.s   IL_00fb

    IL_00e4:  pop
    IL_00e5:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00ea:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_3'()
    IL_00f0:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_00f5:  dup
    IL_00f6:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_3'
    IL_00fb:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0100:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  ldtoken    [mscorlib]System.Type
    IL_010a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0114:  ldc.i4.0
    IL_0115:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0124:  nop
    IL_0125:  ldsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_4'
    IL_012a:  dup
    IL_012b:  brtrue.s   IL_0144

    IL_012d:  pop
    IL_012e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0133:  ldftn      instance class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<TypeOfExpr>b__65_4'()
    IL_0139:  newobj     instance void class [mscorlib]System.Func`1<class [mscorlib]System.Type>::.ctor(object,
                                                                                                         native int)
    IL_013e:  dup
    IL_013f:  stsfld     class [mscorlib]System.Func`1<class [mscorlib]System.Type> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__65_4'
    IL_0144:  ldtoken    int32*
    IL_0149:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014e:  ldtoken    [mscorlib]System.Type
    IL_0153:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0158:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015d:  ldc.i4.0
    IL_015e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0163:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0168:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<class [mscorlib]System.Type>>(!!0,
                                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_016d:  nop
    IL_016e:  ret
  } // end of method ExpressionTrees::TypeOfExpr

  .method public hidebysig static void  AsTypeExpr() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<AsTypeExpr>b__66_0'(object)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>::.ctor(object,
                                                                                                                                                native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_0'
    IL_0020:  ldtoken    [mscorlib]System.Object
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "obj"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_0045:  ldc.i4.1
    IL_0046:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004b:  dup
    IL_004c:  ldc.i4.0
    IL_004d:  ldloc.0
    IL_004e:  stelem.ref
    IL_004f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0054:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass>>(!!0,
                                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0059:  nop
    IL_005a:  ldsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_1'
    IL_005f:  dup
    IL_0060:  brtrue.s   IL_0079

    IL_0062:  pop
    IL_0063:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0068:  ldftn      instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<AsTypeExpr>b__66_1'(object)
    IL_006e:  newobj     instance void class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>::.ctor(object,
                                                                                                                                                                               native int)
    IL_0073:  dup
    IL_0074:  stsfld     class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__66_1'
    IL_0079:  ldtoken    [mscorlib]System.Object
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  ldstr      "obj"
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_008d:  stloc.0
    IL_008e:  ldloc.0
    IL_008f:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_009e:  ldc.i4.1
    IL_009f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a4:  dup
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldloc.0
    IL_00a7:  stelem.ref
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ad:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/GenericClass`1<object>>>(!!0,
                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b2:  nop
    IL_00b3:  ret
  } // end of method ExpressionTrees::AsTypeExpr

  .method public hidebysig static void  IsTypeExpr() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__67_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<IsTypeExpr>b__67_0'(object)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<object,bool>::.ctor(object,
                                                                                         native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<object,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__67_0'
    IL_0020:  ldtoken    [mscorlib]System.Object
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "obj"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  ldc.i4.1
    IL_0046:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004b:  dup
    IL_004c:  ldc.i4.0
    IL_004d:  ldloc.0
    IL_004e:  stelem.ref
    IL_004f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0054:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<object,bool>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0059:  nop
    IL_005a:  ret
  } // end of method ExpressionTrees::IsTypeExpr

  .method public hidebysig static void  UnaryLogicalOperators() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__68_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<UnaryLogicalOperators>b__68_0'(bool)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<bool,bool>::.ctor(object,
                                                                                       native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<bool,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__68_0'
    IL_0020:  ldtoken    [mscorlib]System.Boolean
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003b:  ldc.i4.1
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  dup
    IL_0042:  ldc.i4.0
    IL_0043:  ldloc.0
    IL_0044:  stelem.ref
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<bool,bool>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004f:  nop
    IL_0050:  ret
  } // end of method ExpressionTrees::UnaryLogicalOperators

  .method public hidebysig static void  ConditionalOperator() cil managed
  {
    // Code size       158 (0x9e)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    [mscorlib]System.Boolean
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  ldstr      "a"
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.5
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  ldc.i4.s   10
    IL_002f:  box        [mscorlib]System.Int32
    IL_0034:  ldtoken    [mscorlib]System.Int32
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0048:  ldc.i4.1
    IL_0049:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004e:  dup
    IL_004f:  ldc.i4.0
    IL_0050:  ldloc.0
    IL_0051:  stelem.ref
    IL_0052:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0057:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<bool,int32>(object,
                                                                                                                  class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_005c:  pop
    IL_005d:  ldnull
    IL_005e:  ldtoken    [mscorlib]System.Object
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  ldstr      "a"
    IL_006d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0072:  stloc.0
    IL_0073:  ldloc.0
    IL_0074:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass
    IL_0079:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0088:  ldc.i4.1
    IL_0089:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008e:  dup
    IL_008f:  ldc.i4.0
    IL_0090:  ldloc.0
    IL_0091:  stelem.ref
    IL_0092:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<object,object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0097:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<object,object>(object,
                                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_009c:  pop
    IL_009d:  ret
  } // end of method ExpressionTrees::ConditionalOperator

  .method public hidebysig static void  BinaryLogicalOperators() cil managed
  {
    // Code size       1605 (0x645)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    [mscorlib]System.Int32
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  ldstr      "a"
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0016:  stloc.0
    IL_0017:  ldtoken    [mscorlib]System.Int32
    IL_001c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0021:  ldstr      "b"
    IL_0026:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002b:  stloc.1
    IL_002c:  ldloc.0
    IL_002d:  ldloc.1
    IL_002e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0033:  ldc.i4.2
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0039:  dup
    IL_003a:  ldc.i4.0
    IL_003b:  ldloc.0
    IL_003c:  stelem.ref
    IL_003d:  dup
    IL_003e:  ldc.i4.1
    IL_003f:  ldloc.1
    IL_0040:  stelem.ref
    IL_0041:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0046:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_004b:  pop
    IL_004c:  ldnull
    IL_004d:  ldtoken    [mscorlib]System.Int32
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  ldstr      "a"
    IL_005c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0061:  stloc.1
    IL_0062:  ldtoken    [mscorlib]System.Int32
    IL_0067:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006c:  ldstr      "b"
    IL_0071:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0076:  stloc.0
    IL_0077:  ldloc.1
    IL_0078:  ldloc.0
    IL_0079:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_007e:  ldc.i4.2
    IL_007f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0084:  dup
    IL_0085:  ldc.i4.0
    IL_0086:  ldloc.1
    IL_0087:  stelem.ref
    IL_0088:  dup
    IL_0089:  ldc.i4.1
    IL_008a:  ldloc.0
    IL_008b:  stelem.ref
    IL_008c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0091:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0096:  pop
    IL_0097:  ldnull
    IL_0098:  ldtoken    [mscorlib]System.Int32
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  ldstr      "a"
    IL_00a7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ac:  stloc.0
    IL_00ad:  ldtoken    [mscorlib]System.Int32
    IL_00b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b7:  ldstr      "b"
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00c1:  stloc.1
    IL_00c2:  ldloc.0
    IL_00c3:  ldloc.1
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00c9:  ldc.i4.2
    IL_00ca:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00cf:  dup
    IL_00d0:  ldc.i4.0
    IL_00d1:  ldloc.0
    IL_00d2:  stelem.ref
    IL_00d3:  dup
    IL_00d4:  ldc.i4.1
    IL_00d5:  ldloc.1
    IL_00d6:  stelem.ref
    IL_00d7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00dc:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_00e1:  pop
    IL_00e2:  ldnull
    IL_00e3:  ldtoken    [mscorlib]System.Int32
    IL_00e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ed:  ldstr      "a"
    IL_00f2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f7:  stloc.1
    IL_00f8:  ldtoken    [mscorlib]System.Int32
    IL_00fd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0102:  ldstr      "b"
    IL_0107:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010c:  stloc.0
    IL_010d:  ldloc.1
    IL_010e:  ldloc.0
    IL_010f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0114:  ldc.i4.2
    IL_0115:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011a:  dup
    IL_011b:  ldc.i4.0
    IL_011c:  ldloc.1
    IL_011d:  stelem.ref
    IL_011e:  dup
    IL_011f:  ldc.i4.1
    IL_0120:  ldloc.0
    IL_0121:  stelem.ref
    IL_0122:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0127:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_012c:  pop
    IL_012d:  ldnull
    IL_012e:  ldtoken    [mscorlib]System.Int32
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  ldstr      "a"
    IL_013d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0142:  stloc.0
    IL_0143:  ldtoken    [mscorlib]System.Int32
    IL_0148:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014d:  ldstr      "b"
    IL_0152:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0157:  stloc.1
    IL_0158:  ldloc.0
    IL_0159:  ldloc.1
    IL_015a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_015f:  ldc.i4.2
    IL_0160:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0165:  dup
    IL_0166:  ldc.i4.0
    IL_0167:  ldloc.0
    IL_0168:  stelem.ref
    IL_0169:  dup
    IL_016a:  ldc.i4.1
    IL_016b:  ldloc.1
    IL_016c:  stelem.ref
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0177:  pop
    IL_0178:  ldnull
    IL_0179:  ldtoken    [mscorlib]System.Int32
    IL_017e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0183:  ldstr      "a"
    IL_0188:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_018d:  stloc.1
    IL_018e:  ldtoken    [mscorlib]System.Int32
    IL_0193:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0198:  ldstr      "b"
    IL_019d:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01a2:  stloc.0
    IL_01a3:  ldloc.1
    IL_01a4:  ldloc.0
    IL_01a5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_01aa:  ldc.i4.2
    IL_01ab:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01b0:  dup
    IL_01b1:  ldc.i4.0
    IL_01b2:  ldloc.1
    IL_01b3:  stelem.ref
    IL_01b4:  dup
    IL_01b5:  ldc.i4.1
    IL_01b6:  ldloc.0
    IL_01b7:  stelem.ref
    IL_01b8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01bd:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_01c2:  pop
    IL_01c3:  ldnull
    IL_01c4:  ldtoken    [mscorlib]System.Int32
    IL_01c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ce:  ldstr      "a"
    IL_01d3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d8:  stloc.0
    IL_01d9:  ldtoken    [mscorlib]System.Int32
    IL_01de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e3:  ldstr      "b"
    IL_01e8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01ed:  stloc.1
    IL_01ee:  ldloc.0
    IL_01ef:  ldc.i4.1
    IL_01f0:  box        [mscorlib]System.Int32
    IL_01f5:  ldtoken    [mscorlib]System.Int32
    IL_01fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ff:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0204:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0209:  ldloc.1
    IL_020a:  ldc.i4.2
    IL_020b:  box        [mscorlib]System.Int32
    IL_0210:  ldtoken    [mscorlib]System.Int32
    IL_0215:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_021f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0224:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0229:  ldc.i4.2
    IL_022a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_022f:  dup
    IL_0230:  ldc.i4.0
    IL_0231:  ldloc.0
    IL_0232:  stelem.ref
    IL_0233:  dup
    IL_0234:  ldc.i4.1
    IL_0235:  ldloc.1
    IL_0236:  stelem.ref
    IL_0237:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_023c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0241:  pop
    IL_0242:  ldnull
    IL_0243:  ldtoken    [mscorlib]System.Int32
    IL_0248:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_024d:  ldstr      "a"
    IL_0252:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0257:  stloc.1
    IL_0258:  ldtoken    [mscorlib]System.Int32
    IL_025d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0262:  ldstr      "b"
    IL_0267:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_026c:  stloc.0
    IL_026d:  ldloc.1
    IL_026e:  ldc.i4.1
    IL_026f:  box        [mscorlib]System.Int32
    IL_0274:  ldtoken    [mscorlib]System.Int32
    IL_0279:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_027e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0283:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0288:  ldloc.0
    IL_0289:  ldc.i4.2
    IL_028a:  box        [mscorlib]System.Int32
    IL_028f:  ldtoken    [mscorlib]System.Int32
    IL_0294:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0299:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_029e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_02a3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_02a8:  ldc.i4.2
    IL_02a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02ae:  dup
    IL_02af:  ldc.i4.0
    IL_02b0:  ldloc.1
    IL_02b1:  stelem.ref
    IL_02b2:  dup
    IL_02b3:  ldc.i4.1
    IL_02b4:  ldloc.0
    IL_02b5:  stelem.ref
    IL_02b6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02bb:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_02c0:  pop
    IL_02c1:  ldnull
    IL_02c2:  ldtoken    [mscorlib]System.Int32
    IL_02c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02cc:  ldstr      "a"
    IL_02d1:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02d6:  stloc.0
    IL_02d7:  ldtoken    [mscorlib]System.Int16
    IL_02dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02e1:  ldstr      "b"
    IL_02e6:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02eb:  stloc.1
    IL_02ec:  ldloc.0
    IL_02ed:  ldloc.1
    IL_02ee:  ldtoken    [mscorlib]System.Int32
    IL_02f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02fd:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0302:  ldc.i4.2
    IL_0303:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0308:  dup
    IL_0309:  ldc.i4.0
    IL_030a:  ldloc.0
    IL_030b:  stelem.ref
    IL_030c:  dup
    IL_030d:  ldc.i4.1
    IL_030e:  ldloc.1
    IL_030f:  stelem.ref
    IL_0310:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0315:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int16,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_031a:  pop
    IL_031b:  ldnull
    IL_031c:  ldtoken    [mscorlib]System.UInt16
    IL_0321:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0326:  ldstr      "a"
    IL_032b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0330:  stloc.1
    IL_0331:  ldtoken    [mscorlib]System.Int32
    IL_0336:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033b:  ldstr      "b"
    IL_0340:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0345:  stloc.0
    IL_0346:  ldloc.1
    IL_0347:  ldtoken    [mscorlib]System.Int32
    IL_034c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0351:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0356:  ldloc.0
    IL_0357:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_035c:  ldc.i4.2
    IL_035d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0362:  dup
    IL_0363:  ldc.i4.0
    IL_0364:  ldloc.1
    IL_0365:  stelem.ref
    IL_0366:  dup
    IL_0367:  ldc.i4.1
    IL_0368:  ldloc.0
    IL_0369:  stelem.ref
    IL_036a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint16,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_036f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint16,int32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0374:  pop
    IL_0375:  ldnull
    IL_0376:  ldtoken    [mscorlib]System.Int32
    IL_037b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0380:  ldstr      "a"
    IL_0385:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_038a:  stloc.0
    IL_038b:  ldtoken    [mscorlib]System.Int64
    IL_0390:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0395:  ldstr      "b"
    IL_039a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_039f:  stloc.1
    IL_03a0:  ldloc.0
    IL_03a1:  ldtoken    [mscorlib]System.Int64
    IL_03a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ab:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03b0:  ldloc.1
    IL_03b1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_03b6:  ldc.i4.2
    IL_03b7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03bc:  dup
    IL_03bd:  ldc.i4.0
    IL_03be:  ldloc.0
    IL_03bf:  stelem.ref
    IL_03c0:  dup
    IL_03c1:  ldc.i4.1
    IL_03c2:  ldloc.1
    IL_03c3:  stelem.ref
    IL_03c4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03c9:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_03ce:  pop
    IL_03cf:  ldnull
    IL_03d0:  ldtoken    [mscorlib]System.UInt64
    IL_03d5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03da:  ldstr      "a"
    IL_03df:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03e4:  stloc.1
    IL_03e5:  ldtoken    [mscorlib]System.UInt32
    IL_03ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ef:  ldstr      "b"
    IL_03f4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03f9:  stloc.0
    IL_03fa:  ldloc.1
    IL_03fb:  ldloc.0
    IL_03fc:  ldtoken    [mscorlib]System.UInt64
    IL_0401:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0406:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_040b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0410:  ldc.i4.2
    IL_0411:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0416:  dup
    IL_0417:  ldc.i4.0
    IL_0418:  ldloc.1
    IL_0419:  stelem.ref
    IL_041a:  dup
    IL_041b:  ldc.i4.1
    IL_041c:  ldloc.0
    IL_041d:  stelem.ref
    IL_041e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<uint64,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                 class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0423:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<uint64,uint32,bool>(object,
                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0428:  pop
    IL_0429:  ldnull
    IL_042a:  ldtoken    [mscorlib]System.Int32
    IL_042f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0434:  ldstr      "a"
    IL_0439:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_043e:  stloc.0
    IL_043f:  ldtoken    [mscorlib]System.UInt32
    IL_0444:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0449:  ldstr      "b"
    IL_044e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0453:  stloc.1
    IL_0454:  ldloc.0
    IL_0455:  ldtoken    [mscorlib]System.Int64
    IL_045a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_045f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0464:  ldloc.1
    IL_0465:  ldtoken    [mscorlib]System.Int64
    IL_046a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_046f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0474:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression)
    IL_0479:  ldc.i4.2
    IL_047a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_047f:  dup
    IL_0480:  ldc.i4.0
    IL_0481:  ldloc.0
    IL_0482:  stelem.ref
    IL_0483:  dup
    IL_0484:  ldc.i4.1
    IL_0485:  ldloc.1
    IL_0486:  stelem.ref
    IL_0487:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,uint32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_048c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,uint32,bool>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0491:  pop
    IL_0492:  ldnull
    IL_0493:  ldtoken    [mscorlib]System.Int32
    IL_0498:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_049d:  ldstr      "a"
    IL_04a2:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04a7:  stloc.1
    IL_04a8:  ldtoken    [mscorlib]System.Int64
    IL_04ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04b2:  ldstr      "b"
    IL_04b7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04bc:  stloc.0
    IL_04bd:  ldloc.1
    IL_04be:  ldtoken    [mscorlib]System.Int64
    IL_04c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04c8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04cd:  ldloc.0
    IL_04ce:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_04d3:  ldc.i4.2
    IL_04d4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04d9:  dup
    IL_04da:  ldc.i4.0
    IL_04db:  ldloc.1
    IL_04dc:  stelem.ref
    IL_04dd:  dup
    IL_04de:  ldc.i4.1
    IL_04df:  ldloc.0
    IL_04e0:  stelem.ref
    IL_04e1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04e6:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_04eb:  pop
    IL_04ec:  ldnull
    IL_04ed:  ldtoken    [mscorlib]System.Int16
    IL_04f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04f7:  ldstr      "a"
    IL_04fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0501:  stloc.0
    IL_0502:  ldtoken    [mscorlib]System.Int64
    IL_0507:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_050c:  ldstr      "b"
    IL_0511:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0516:  stloc.1
    IL_0517:  ldloc.0
    IL_0518:  ldtoken    [mscorlib]System.Int64
    IL_051d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0522:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0527:  ldloc.1
    IL_0528:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThanOrEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_052d:  ldc.i4.2
    IL_052e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0533:  dup
    IL_0534:  ldc.i4.0
    IL_0535:  ldloc.0
    IL_0536:  stelem.ref
    IL_0537:  dup
    IL_0538:  ldc.i4.1
    IL_0539:  ldloc.1
    IL_053a:  stelem.ref
    IL_053b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int64,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0540:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int16,int64,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0545:  pop
    IL_0546:  ldnull
    IL_0547:  ldtoken    [mscorlib]System.Int32
    IL_054c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0551:  ldstr      "a"
    IL_0556:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_055b:  stloc.1
    IL_055c:  ldtoken    [mscorlib]System.Int32
    IL_0561:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0566:  ldstr      "b"
    IL_056b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0570:  stloc.0
    IL_0571:  ldloc.1
    IL_0572:  ldc.i4.1
    IL_0573:  box        [mscorlib]System.Int32
    IL_0578:  ldtoken    [mscorlib]System.Int32
    IL_057d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0582:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0587:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_058c:  ldloc.0
    IL_058d:  ldc.i4.2
    IL_058e:  box        [mscorlib]System.Int32
    IL_0593:  ldtoken    [mscorlib]System.Int32
    IL_0598:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_059d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_05a2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_05a7:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_05ac:  ldc.i4.2
    IL_05ad:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05b2:  dup
    IL_05b3:  ldc.i4.0
    IL_05b4:  ldloc.1
    IL_05b5:  stelem.ref
    IL_05b6:  dup
    IL_05b7:  ldc.i4.1
    IL_05b8:  ldloc.0
    IL_05b9:  stelem.ref
    IL_05ba:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05bf:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_05c4:  pop
    IL_05c5:  ldnull
    IL_05c6:  ldtoken    [mscorlib]System.Int32
    IL_05cb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05d0:  ldstr      "a"
    IL_05d5:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05da:  stloc.0
    IL_05db:  ldtoken    [mscorlib]System.Int32
    IL_05e0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05e5:  ldstr      "b"
    IL_05ea:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_05ef:  stloc.1
    IL_05f0:  ldloc.0
    IL_05f1:  ldc.i4.1
    IL_05f2:  box        [mscorlib]System.Int32
    IL_05f7:  ldtoken    [mscorlib]System.Int32
    IL_05fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0601:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0606:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_060b:  ldloc.1
    IL_060c:  ldc.i4.2
    IL_060d:  box        [mscorlib]System.Int32
    IL_0612:  ldtoken    [mscorlib]System.Int32
    IL_0617:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_061c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0621:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0626:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_062b:  ldc.i4.2
    IL_062c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0631:  dup
    IL_0632:  ldc.i4.0
    IL_0633:  ldloc.0
    IL_0634:  stelem.ref
    IL_0635:  dup
    IL_0636:  ldc.i4.1
    IL_0637:  ldloc.1
    IL_0638:  stelem.ref
    IL_0639:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_063e:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32,int32,bool>(object,
                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<!!0,!!1,!!2>>)
    IL_0643:  pop
    IL_0644:  ret
  } // end of method ExpressionTrees::BinaryLogicalOperators

  .method public hidebysig static void  UnaryArithmeticOperators() cil managed
  {
    // Code size       155 (0x9b)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<UnaryArithmeticOperators>b__71_0'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.1
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003c:  dup
    IL_003d:  ldc.i4.0
    IL_003e:  ldloc.0
    IL_003f:  stelem.ref
    IL_0040:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0045:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004a:  nop
    IL_004b:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_1'
    IL_0050:  dup
    IL_0051:  brtrue.s   IL_006a

    IL_0053:  pop
    IL_0054:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0059:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<UnaryArithmeticOperators>b__71_1'(int32)
    IL_005f:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_0064:  dup
    IL_0065:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__71_1'
    IL_006a:  ldtoken    [mscorlib]System.Int32
    IL_006f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0074:  ldstr      "a"
    IL_0079:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007e:  stloc.0
    IL_007f:  ldloc.0
    IL_0080:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0085:  ldc.i4.1
    IL_0086:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008b:  dup
    IL_008c:  ldc.i4.0
    IL_008d:  ldloc.0
    IL_008e:  stelem.ref
    IL_008f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0094:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0099:  nop
    IL_009a:  ret
  } // end of method ExpressionTrees::UnaryArithmeticOperators

  .method public hidebysig static void  BinaryArithmeticOperators() cil managed
  {
    // Code size       1727 (0x6bf)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_0'(int32,
                                                                                                                                                  int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldtoken    [mscorlib]System.Int32
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  ldstr      "b"
    IL_0044:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0049:  stloc.1
    IL_004a:  ldloc.0
    IL_004b:  ldloc.1
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.2
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  dup
    IL_0058:  ldc.i4.0
    IL_0059:  ldloc.0
    IL_005a:  stelem.ref
    IL_005b:  dup
    IL_005c:  ldc.i4.1
    IL_005d:  ldloc.1
    IL_005e:  stelem.ref
    IL_005f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0064:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0069:  nop
    IL_006a:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_1'
    IL_006f:  dup
    IL_0070:  brtrue.s   IL_0089

    IL_0072:  pop
    IL_0073:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0078:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_1'(int32,
                                                                                                                                                  int32)
    IL_007e:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0083:  dup
    IL_0084:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_1'
    IL_0089:  ldtoken    [mscorlib]System.Int32
    IL_008e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0093:  ldstr      "a"
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_009d:  stloc.1
    IL_009e:  ldtoken    [mscorlib]System.Int32
    IL_00a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a8:  ldstr      "b"
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00b2:  stloc.0
    IL_00b3:  ldloc.1
    IL_00b4:  ldloc.0
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00ba:  ldc.i4.2
    IL_00bb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00c0:  dup
    IL_00c1:  ldc.i4.0
    IL_00c2:  ldloc.1
    IL_00c3:  stelem.ref
    IL_00c4:  dup
    IL_00c5:  ldc.i4.1
    IL_00c6:  ldloc.0
    IL_00c7:  stelem.ref
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00cd:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00d2:  nop
    IL_00d3:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_2'
    IL_00d8:  dup
    IL_00d9:  brtrue.s   IL_00f2

    IL_00db:  pop
    IL_00dc:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00e1:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_2'(int32,
                                                                                                                                                  int32)
    IL_00e7:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00ec:  dup
    IL_00ed:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_2'
    IL_00f2:  ldtoken    [mscorlib]System.Int32
    IL_00f7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fc:  ldstr      "a"
    IL_0101:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0106:  stloc.0
    IL_0107:  ldtoken    [mscorlib]System.Int32
    IL_010c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0111:  ldstr      "b"
    IL_0116:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_011b:  stloc.1
    IL_011c:  ldloc.0
    IL_011d:  ldloc.1
    IL_011e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0123:  ldc.i4.2
    IL_0124:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0129:  dup
    IL_012a:  ldc.i4.0
    IL_012b:  ldloc.0
    IL_012c:  stelem.ref
    IL_012d:  dup
    IL_012e:  ldc.i4.1
    IL_012f:  ldloc.1
    IL_0130:  stelem.ref
    IL_0131:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0136:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_013b:  nop
    IL_013c:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_3'
    IL_0141:  dup
    IL_0142:  brtrue.s   IL_015b

    IL_0144:  pop
    IL_0145:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_014a:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_3'(int32,
                                                                                                                                                  int32)
    IL_0150:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0155:  dup
    IL_0156:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_3'
    IL_015b:  ldtoken    [mscorlib]System.Int32
    IL_0160:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0165:  ldstr      "a"
    IL_016a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_016f:  stloc.1
    IL_0170:  ldtoken    [mscorlib]System.Int32
    IL_0175:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017a:  ldstr      "b"
    IL_017f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0184:  stloc.0
    IL_0185:  ldloc.1
    IL_0186:  ldloc.0
    IL_0187:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_018c:  ldc.i4.2
    IL_018d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0192:  dup
    IL_0193:  ldc.i4.0
    IL_0194:  ldloc.1
    IL_0195:  stelem.ref
    IL_0196:  dup
    IL_0197:  ldc.i4.1
    IL_0198:  ldloc.0
    IL_0199:  stelem.ref
    IL_019a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_019f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01a4:  nop
    IL_01a5:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_4'
    IL_01aa:  dup
    IL_01ab:  brtrue.s   IL_01c4

    IL_01ad:  pop
    IL_01ae:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01b3:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_4'(int32,
                                                                                                                                                  int32)
    IL_01b9:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_01be:  dup
    IL_01bf:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_4'
    IL_01c4:  ldtoken    [mscorlib]System.Int32
    IL_01c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ce:  ldstr      "a"
    IL_01d3:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d8:  stloc.0
    IL_01d9:  ldtoken    [mscorlib]System.Int32
    IL_01de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e3:  ldstr      "b"
    IL_01e8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01ed:  stloc.1
    IL_01ee:  ldloc.0
    IL_01ef:  ldloc.1
    IL_01f0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_01f5:  ldc.i4.2
    IL_01f6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01fb:  dup
    IL_01fc:  ldc.i4.0
    IL_01fd:  ldloc.0
    IL_01fe:  stelem.ref
    IL_01ff:  dup
    IL_0200:  ldc.i4.1
    IL_0201:  ldloc.1
    IL_0202:  stelem.ref
    IL_0203:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0208:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_020d:  nop
    IL_020e:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_5'
    IL_0213:  dup
    IL_0214:  brtrue.s   IL_022d

    IL_0216:  pop
    IL_0217:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_021c:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_5'(int64,
                                                                                                                                                  int32)
    IL_0222:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0227:  dup
    IL_0228:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_5'
    IL_022d:  ldtoken    [mscorlib]System.Int64
    IL_0232:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0237:  ldstr      "a"
    IL_023c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0241:  stloc.1
    IL_0242:  ldtoken    [mscorlib]System.Int32
    IL_0247:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_024c:  ldstr      "b"
    IL_0251:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0256:  stloc.0
    IL_0257:  ldloc.1
    IL_0258:  ldloc.0
    IL_0259:  ldtoken    [mscorlib]System.Int64
    IL_025e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0263:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0268:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_026d:  ldc.i4.2
    IL_026e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0273:  dup
    IL_0274:  ldc.i4.0
    IL_0275:  ldloc.1
    IL_0276:  stelem.ref
    IL_0277:  dup
    IL_0278:  ldc.i4.1
    IL_0279:  ldloc.0
    IL_027a:  stelem.ref
    IL_027b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0280:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0285:  nop
    IL_0286:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_6'
    IL_028b:  dup
    IL_028c:  brtrue.s   IL_02a5

    IL_028e:  pop
    IL_028f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0294:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_6'(int64,
                                                                                                                                                  int32)
    IL_029a:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_029f:  dup
    IL_02a0:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_6'
    IL_02a5:  ldtoken    [mscorlib]System.Int64
    IL_02aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02af:  ldstr      "a"
    IL_02b4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02b9:  stloc.0
    IL_02ba:  ldtoken    [mscorlib]System.Int32
    IL_02bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02c4:  ldstr      "b"
    IL_02c9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_02ce:  stloc.1
    IL_02cf:  ldloc.0
    IL_02d0:  ldloc.1
    IL_02d1:  ldtoken    [mscorlib]System.Int64
    IL_02d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02db:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_02e0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_02e5:  ldc.i4.2
    IL_02e6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_02eb:  dup
    IL_02ec:  ldc.i4.0
    IL_02ed:  ldloc.0
    IL_02ee:  stelem.ref
    IL_02ef:  dup
    IL_02f0:  ldc.i4.1
    IL_02f1:  ldloc.1
    IL_02f2:  stelem.ref
    IL_02f3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_02f8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_02fd:  nop
    IL_02fe:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_7'
    IL_0303:  dup
    IL_0304:  brtrue.s   IL_031d

    IL_0306:  pop
    IL_0307:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_030c:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_7'(int64,
                                                                                                                                                  int32)
    IL_0312:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0317:  dup
    IL_0318:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_7'
    IL_031d:  ldtoken    [mscorlib]System.Int64
    IL_0322:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0327:  ldstr      "a"
    IL_032c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0331:  stloc.1
    IL_0332:  ldtoken    [mscorlib]System.Int32
    IL_0337:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033c:  ldstr      "b"
    IL_0341:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0346:  stloc.0
    IL_0347:  ldloc.1
    IL_0348:  ldloc.0
    IL_0349:  ldtoken    [mscorlib]System.Int64
    IL_034e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0353:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0358:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_035d:  ldc.i4.2
    IL_035e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0363:  dup
    IL_0364:  ldc.i4.0
    IL_0365:  ldloc.1
    IL_0366:  stelem.ref
    IL_0367:  dup
    IL_0368:  ldc.i4.1
    IL_0369:  ldloc.0
    IL_036a:  stelem.ref
    IL_036b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0370:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0375:  nop
    IL_0376:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_8'
    IL_037b:  dup
    IL_037c:  brtrue.s   IL_0395

    IL_037e:  pop
    IL_037f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0384:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_8'(int64,
                                                                                                                                                  int32)
    IL_038a:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_038f:  dup
    IL_0390:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_8'
    IL_0395:  ldtoken    [mscorlib]System.Int64
    IL_039a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_039f:  ldstr      "a"
    IL_03a4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03a9:  stloc.0
    IL_03aa:  ldtoken    [mscorlib]System.Int32
    IL_03af:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03b4:  ldstr      "b"
    IL_03b9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_03be:  stloc.1
    IL_03bf:  ldloc.0
    IL_03c0:  ldloc.1
    IL_03c1:  ldtoken    [mscorlib]System.Int64
    IL_03c6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03cb:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_03d0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_03d5:  ldc.i4.2
    IL_03d6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_03db:  dup
    IL_03dc:  ldc.i4.0
    IL_03dd:  ldloc.0
    IL_03de:  stelem.ref
    IL_03df:  dup
    IL_03e0:  ldc.i4.1
    IL_03e1:  ldloc.1
    IL_03e2:  stelem.ref
    IL_03e3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_03e8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_03ed:  nop
    IL_03ee:  ldsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_9'
    IL_03f3:  dup
    IL_03f4:  brtrue.s   IL_040d

    IL_03f6:  pop
    IL_03f7:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_03fc:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_9'(int64,
                                                                                                                                                  int32)
    IL_0402:  newobj     instance void class [mscorlib]System.Func`3<int64,int32,int64>::.ctor(object,
                                                                                               native int)
    IL_0407:  dup
    IL_0408:  stsfld     class [mscorlib]System.Func`3<int64,int32,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_9'
    IL_040d:  ldtoken    [mscorlib]System.Int64
    IL_0412:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0417:  ldstr      "a"
    IL_041c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0421:  stloc.1
    IL_0422:  ldtoken    [mscorlib]System.Int32
    IL_0427:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_042c:  ldstr      "b"
    IL_0431:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0436:  stloc.0
    IL_0437:  ldloc.1
    IL_0438:  ldloc.0
    IL_0439:  ldtoken    [mscorlib]System.Int64
    IL_043e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0443:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0448:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_044d:  ldc.i4.2
    IL_044e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0453:  dup
    IL_0454:  ldc.i4.0
    IL_0455:  ldloc.1
    IL_0456:  stelem.ref
    IL_0457:  dup
    IL_0458:  ldc.i4.1
    IL_0459:  ldloc.0
    IL_045a:  stelem.ref
    IL_045b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int64,int32,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0460:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int64,int32,int64>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0465:  nop
    IL_0466:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_10'
    IL_046b:  dup
    IL_046c:  brtrue.s   IL_0485

    IL_046e:  pop
    IL_046f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0474:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_10'(int16,
                                                                                                                                                   int32)
    IL_047a:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_047f:  dup
    IL_0480:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_10'
    IL_0485:  ldtoken    [mscorlib]System.Int16
    IL_048a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_048f:  ldstr      "a"
    IL_0494:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0499:  stloc.0
    IL_049a:  ldtoken    [mscorlib]System.Int32
    IL_049f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04a4:  ldstr      "b"
    IL_04a9:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_04ae:  stloc.1
    IL_04af:  ldloc.0
    IL_04b0:  ldtoken    [mscorlib]System.Int32
    IL_04b5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_04ba:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_04bf:  ldloc.1
    IL_04c0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_04c5:  ldc.i4.2
    IL_04c6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_04cb:  dup
    IL_04cc:  ldc.i4.0
    IL_04cd:  ldloc.0
    IL_04ce:  stelem.ref
    IL_04cf:  dup
    IL_04d0:  ldc.i4.1
    IL_04d1:  ldloc.1
    IL_04d2:  stelem.ref
    IL_04d3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_04d8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_04dd:  nop
    IL_04de:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_11'
    IL_04e3:  dup
    IL_04e4:  brtrue.s   IL_04fd

    IL_04e6:  pop
    IL_04e7:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_04ec:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_11'(int32,
                                                                                                                                                   int16)
    IL_04f2:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_04f7:  dup
    IL_04f8:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_11'
    IL_04fd:  ldtoken    [mscorlib]System.Int32
    IL_0502:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0507:  ldstr      "a"
    IL_050c:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0511:  stloc.1
    IL_0512:  ldtoken    [mscorlib]System.Int16
    IL_0517:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_051c:  ldstr      "b"
    IL_0521:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0526:  stloc.0
    IL_0527:  ldloc.1
    IL_0528:  ldloc.0
    IL_0529:  ldtoken    [mscorlib]System.Int32
    IL_052e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0533:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0538:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Subtract(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_053d:  ldc.i4.2
    IL_053e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0543:  dup
    IL_0544:  ldc.i4.0
    IL_0545:  ldloc.1
    IL_0546:  stelem.ref
    IL_0547:  dup
    IL_0548:  ldc.i4.1
    IL_0549:  ldloc.0
    IL_054a:  stelem.ref
    IL_054b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0550:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0555:  nop
    IL_0556:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_12'
    IL_055b:  dup
    IL_055c:  brtrue.s   IL_0575

    IL_055e:  pop
    IL_055f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0564:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_12'(int16,
                                                                                                                                                   int32)
    IL_056a:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_056f:  dup
    IL_0570:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_12'
    IL_0575:  ldtoken    [mscorlib]System.Int16
    IL_057a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_057f:  ldstr      "a"
    IL_0584:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0589:  stloc.0
    IL_058a:  ldtoken    [mscorlib]System.Int32
    IL_058f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0594:  ldstr      "b"
    IL_0599:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_059e:  stloc.1
    IL_059f:  ldloc.0
    IL_05a0:  ldtoken    [mscorlib]System.Int32
    IL_05a5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05aa:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_05af:  ldloc.1
    IL_05b0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_05b5:  ldc.i4.2
    IL_05b6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_05bb:  dup
    IL_05bc:  ldc.i4.0
    IL_05bd:  ldloc.0
    IL_05be:  stelem.ref
    IL_05bf:  dup
    IL_05c0:  ldc.i4.1
    IL_05c1:  ldloc.1
    IL_05c2:  stelem.ref
    IL_05c3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_05c8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_05cd:  nop
    IL_05ce:  ldsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_13'
    IL_05d3:  dup
    IL_05d4:  brtrue.s   IL_05ed

    IL_05d6:  pop
    IL_05d7:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_05dc:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_13'(int32,
                                                                                                                                                   int16)
    IL_05e2:  newobj     instance void class [mscorlib]System.Func`3<int32,int16,int32>::.ctor(object,
                                                                                               native int)
    IL_05e7:  dup
    IL_05e8:  stsfld     class [mscorlib]System.Func`3<int32,int16,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_13'
    IL_05ed:  ldtoken    [mscorlib]System.Int32
    IL_05f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_05f7:  ldstr      "a"
    IL_05fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0601:  stloc.1
    IL_0602:  ldtoken    [mscorlib]System.Int16
    IL_0607:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_060c:  ldstr      "b"
    IL_0611:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0616:  stloc.0
    IL_0617:  ldloc.1
    IL_0618:  ldloc.0
    IL_0619:  ldtoken    [mscorlib]System.Int32
    IL_061e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0623:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0628:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Divide(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_062d:  ldc.i4.2
    IL_062e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0633:  dup
    IL_0634:  ldc.i4.0
    IL_0635:  ldloc.1
    IL_0636:  stelem.ref
    IL_0637:  dup
    IL_0638:  ldc.i4.1
    IL_0639:  ldloc.0
    IL_063a:  stelem.ref
    IL_063b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int16,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0640:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int16,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0645:  nop
    IL_0646:  ldsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_14'
    IL_064b:  dup
    IL_064c:  brtrue.s   IL_0665

    IL_064e:  pop
    IL_064f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0654:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BinaryArithmeticOperators>b__72_14'(int16,
                                                                                                                                                   int32)
    IL_065a:  newobj     instance void class [mscorlib]System.Func`3<int16,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_065f:  dup
    IL_0660:  stsfld     class [mscorlib]System.Func`3<int16,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__72_14'
    IL_0665:  ldtoken    [mscorlib]System.Int16
    IL_066a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_066f:  ldstr      "a"
    IL_0674:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0679:  stloc.0
    IL_067a:  ldtoken    [mscorlib]System.Int32
    IL_067f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0684:  ldstr      "b"
    IL_0689:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_068e:  stloc.1
    IL_068f:  ldloc.0
    IL_0690:  ldtoken    [mscorlib]System.Int32
    IL_0695:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_069a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_069f:  ldloc.1
    IL_06a0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_06a5:  ldc.i4.2
    IL_06a6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_06ab:  dup
    IL_06ac:  ldc.i4.0
    IL_06ad:  ldloc.0
    IL_06ae:  stelem.ref
    IL_06af:  dup
    IL_06b0:  ldc.i4.1
    IL_06b1:  ldloc.1
    IL_06b2:  stelem.ref
    IL_06b3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int16,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_06b8:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int16,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_06bd:  nop
    IL_06be:  ret
  } // end of method ExpressionTrees::BinaryArithmeticOperators

  .method public hidebysig static void  BitOperators() cil managed
  {
    // Code size       396 (0x18c)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_0'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003b:  ldc.i4.1
    IL_003c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0041:  dup
    IL_0042:  ldc.i4.0
    IL_0043:  ldloc.0
    IL_0044:  stelem.ref
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_004f:  nop
    IL_0050:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_1'
    IL_0055:  dup
    IL_0056:  brtrue.s   IL_006f

    IL_0058:  pop
    IL_0059:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_005e:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_1'(int32,
                                                                                                                                     int32)
    IL_0064:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0069:  dup
    IL_006a:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_1'
    IL_006f:  ldtoken    [mscorlib]System.Int32
    IL_0074:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0079:  ldstr      "a"
    IL_007e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0083:  stloc.0
    IL_0084:  ldtoken    [mscorlib]System.Int32
    IL_0089:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008e:  ldstr      "b"
    IL_0093:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0098:  stloc.1
    IL_0099:  ldloc.0
    IL_009a:  ldloc.1
    IL_009b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::And(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00a0:  ldc.i4.2
    IL_00a1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a6:  dup
    IL_00a7:  ldc.i4.0
    IL_00a8:  ldloc.0
    IL_00a9:  stelem.ref
    IL_00aa:  dup
    IL_00ab:  ldc.i4.1
    IL_00ac:  ldloc.1
    IL_00ad:  stelem.ref
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b8:  nop
    IL_00b9:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_2'
    IL_00be:  dup
    IL_00bf:  brtrue.s   IL_00d8

    IL_00c1:  pop
    IL_00c2:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00c7:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_2'(int32,
                                                                                                                                     int32)
    IL_00cd:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_00d2:  dup
    IL_00d3:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_2'
    IL_00d8:  ldtoken    [mscorlib]System.Int32
    IL_00dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e2:  ldstr      "a"
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00ec:  stloc.1
    IL_00ed:  ldtoken    [mscorlib]System.Int32
    IL_00f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f7:  ldstr      "b"
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0101:  stloc.0
    IL_0102:  ldloc.1
    IL_0103:  ldloc.0
    IL_0104:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Or(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression)
    IL_0109:  ldc.i4.2
    IL_010a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010f:  dup
    IL_0110:  ldc.i4.0
    IL_0111:  ldloc.1
    IL_0112:  stelem.ref
    IL_0113:  dup
    IL_0114:  ldc.i4.1
    IL_0115:  ldloc.0
    IL_0116:  stelem.ref
    IL_0117:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_011c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0121:  nop
    IL_0122:  ldsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_3'
    IL_0127:  dup
    IL_0128:  brtrue.s   IL_0141

    IL_012a:  pop
    IL_012b:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0130:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<BitOperators>b__73_3'(int32,
                                                                                                                                     int32)
    IL_0136:  newobj     instance void class [mscorlib]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_013b:  dup
    IL_013c:  stsfld     class [mscorlib]System.Func`3<int32,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__73_3'
    IL_0141:  ldtoken    [mscorlib]System.Int32
    IL_0146:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014b:  ldstr      "a"
    IL_0150:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0155:  stloc.0
    IL_0156:  ldtoken    [mscorlib]System.Int32
    IL_015b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0160:  ldstr      "b"
    IL_0165:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_016a:  stloc.1
    IL_016b:  ldloc.0
    IL_016c:  ldloc.1
    IL_016d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ExclusiveOr(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0172:  ldc.i4.2
    IL_0173:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0178:  dup
    IL_0179:  ldc.i4.0
    IL_017a:  ldloc.0
    IL_017b:  stelem.ref
    IL_017c:  dup
    IL_017d:  ldc.i4.1
    IL_017e:  ldloc.1
    IL_017f:  stelem.ref
    IL_0180:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0185:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`3<int32,int32,int32>>(!!0,
                                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_018a:  nop
    IL_018b:  ret
  } // end of method ExpressionTrees::BitOperators

  .method public hidebysig static void  ShiftOperators() cil managed
  {
    // Code size       402 (0x192)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_0'(int32)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "a"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.2
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0050:  ldc.i4.1
    IL_0051:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0056:  dup
    IL_0057:  ldc.i4.0
    IL_0058:  ldloc.0
    IL_0059:  stelem.ref
    IL_005a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0064:  nop
    IL_0065:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_1'
    IL_006a:  dup
    IL_006b:  brtrue.s   IL_0084

    IL_006d:  pop
    IL_006e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0073:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_1'(int32)
    IL_0079:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_007e:  dup
    IL_007f:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_1'
    IL_0084:  ldtoken    [mscorlib]System.Int32
    IL_0089:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008e:  ldstr      "a"
    IL_0093:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0098:  stloc.0
    IL_0099:  ldloc.0
    IL_009a:  ldc.i4.2
    IL_009b:  box        [mscorlib]System.Int32
    IL_00a0:  ldtoken    [mscorlib]System.Int32
    IL_00a5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00af:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_00b4:  ldc.i4.1
    IL_00b5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ba:  dup
    IL_00bb:  ldc.i4.0
    IL_00bc:  ldloc.0
    IL_00bd:  stelem.ref
    IL_00be:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00c8:  nop
    IL_00c9:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_2'
    IL_00ce:  dup
    IL_00cf:  brtrue.s   IL_00e8

    IL_00d1:  pop
    IL_00d2:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00d7:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_2'(int64)
    IL_00dd:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_00e2:  dup
    IL_00e3:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_2'
    IL_00e8:  ldtoken    [mscorlib]System.Int64
    IL_00ed:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f2:  ldstr      "a"
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00fc:  stloc.0
    IL_00fd:  ldloc.0
    IL_00fe:  ldc.i4.2
    IL_00ff:  box        [mscorlib]System.Int32
    IL_0104:  ldtoken    [mscorlib]System.Int32
    IL_0109:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0113:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::RightShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_0118:  ldc.i4.1
    IL_0119:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011e:  dup
    IL_011f:  ldc.i4.0
    IL_0120:  ldloc.0
    IL_0121:  stelem.ref
    IL_0122:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0127:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_012c:  nop
    IL_012d:  ldsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_3'
    IL_0132:  dup
    IL_0133:  brtrue.s   IL_014c

    IL_0135:  pop
    IL_0136:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_013b:  ldftn      instance int64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ShiftOperators>b__74_3'(int64)
    IL_0141:  newobj     instance void class [mscorlib]System.Func`2<int64,int64>::.ctor(object,
                                                                                         native int)
    IL_0146:  dup
    IL_0147:  stsfld     class [mscorlib]System.Func`2<int64,int64> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__74_3'
    IL_014c:  ldtoken    [mscorlib]System.Int64
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  ldstr      "a"
    IL_015b:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0160:  stloc.0
    IL_0161:  ldloc.0
    IL_0162:  ldc.i4.2
    IL_0163:  box        [mscorlib]System.Int32
    IL_0168:  ldtoken    [mscorlib]System.Int32
    IL_016d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0172:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0177:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LeftShift(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression)
    IL_017c:  ldc.i4.1
    IL_017d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0182:  dup
    IL_0183:  ldc.i4.0
    IL_0184:  ldloc.0
    IL_0185:  stelem.ref
    IL_0186:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int64,int64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int64,int64>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0190:  nop
    IL_0191:  ret
  } // end of method ExpressionTrees::ShiftOperators

  .method public hidebysig static void  SimpleExpressions() cil managed
  {
    // Code size       145 (0x91)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<SimpleExpressions>b__75_0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_0'
    IL_0020:  ldc.i4.0
    IL_0021:  box        [mscorlib]System.Int32
    IL_0026:  ldtoken    [mscorlib]System.Int32
    IL_002b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0030:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0035:  ldc.i4.0
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0040:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0045:  nop
    IL_0046:  ldsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_1'
    IL_004b:  dup
    IL_004c:  brtrue.s   IL_0065

    IL_004e:  pop
    IL_004f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0054:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<SimpleExpressions>b__75_1'(int32)
    IL_005a:  newobj     instance void class [mscorlib]System.Func`2<int32,int32>::.ctor(object,
                                                                                         native int)
    IL_005f:  dup
    IL_0060:  stsfld     class [mscorlib]System.Func`2<int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__75_1'
    IL_0065:  ldtoken    [mscorlib]System.Int32
    IL_006a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006f:  ldstr      "a"
    IL_0074:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0079:  stloc.0
    IL_007a:  ldloc.0
    IL_007b:  ldc.i4.1
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  dup
    IL_0082:  ldc.i4.0
    IL_0083:  ldloc.0
    IL_0084:  stelem.ref
    IL_0085:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,int32>>(!!0,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008f:  nop
    IL_0090:  ret
  } // end of method ExpressionTrees::SimpleExpressions

  .method public hidebysig static void  Capturing() cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0' V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.5
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::captured
    IL_000e:  ldloc.0
    IL_000f:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::'<Capturing>b__0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<int32>::.ctor(object,
                                                                                   native int)
    IL_001a:  ldloc.0
    IL_001b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002a:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c__DisplayClass76_0'::captured
    IL_002f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0039:  ldc.i4.0
    IL_003a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32>>(!!0,
                                                                                                                                        class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0049:  nop
    IL_004a:  ret
  } // end of method ExpressionTrees::Capturing

  .method public hidebysig static void  FieldAndPropertyAccess() cil managed
  {
    // Code size       433 (0x1b1)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldc.i4.1
    IL_0003:  box        [mscorlib]System.Int32
    IL_0008:  ldtoken    [mscorlib]System.Int32
    IL_000d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0012:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0017:  ldc.i4.0
    IL_0018:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_001d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0022:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0027:  pop
    IL_0028:  ldnull
    IL_0029:  ldnull
    IL_002a:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticField
    IL_002f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0039:  ldc.i4.0
    IL_003a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0049:  pop
    IL_004a:  ldnull
    IL_004b:  ldnull
    IL_004c:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::StaticReadonlyField
    IL_0051:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006b:  pop
    IL_006c:  ldnull
    IL_006d:  ldnull
    IL_006e:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticProperty()
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0082:  ldc.i4.0
    IL_0083:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0088:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0092:  pop
    IL_0093:  ldnull
    IL_0094:  ldnull
    IL_0095:  ldtoken    method int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_StaticReadonlyProperty()
    IL_009a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a9:  ldc.i4.0
    IL_00aa:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00af:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b4:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<int32>(object,
                                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b9:  pop
    IL_00ba:  ldnull
    IL_00bb:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00c0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c5:  ldstr      "a"
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00cf:  stloc.0
    IL_00d0:  ldloc.0
    IL_00d1:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_00d6:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00db:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00e0:  ldc.i4.1
    IL_00e1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e6:  dup
    IL_00e7:  ldc.i4.0
    IL_00e8:  ldloc.0
    IL_00e9:  stelem.ref
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ef:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_00f4:  pop
    IL_00f5:  ldnull
    IL_00f6:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_00fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0100:  ldstr      "a"
    IL_0105:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_010a:  stloc.0
    IL_010b:  ldloc.0
    IL_010c:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_Property()
    IL_0111:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0116:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0120:  ldc.i4.1
    IL_0121:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0126:  dup
    IL_0127:  ldc.i4.0
    IL_0128:  ldloc.0
    IL_0129:  stelem.ref
    IL_012a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_012f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0134:  pop
    IL_0135:  ldnull
    IL_0136:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  ldstr      "a"
    IL_0145:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_014a:  stloc.0
    IL_014b:  ldloc.0
    IL_014c:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0151:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0156:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_015b:  ldc.i4.1
    IL_015c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0161:  dup
    IL_0162:  ldc.i4.0
    IL_0163:  ldloc.0
    IL_0164:  stelem.ref
    IL_0165:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_016f:  pop
    IL_0170:  ldnull
    IL_0171:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0176:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017b:  ldstr      "a"
    IL_0180:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0185:  stloc.0
    IL_0186:  ldloc.0
    IL_0187:  ldtoken    method instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::get_ReadonlyProperty()
    IL_018c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0191:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0196:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_019b:  ldc.i4.1
    IL_019c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01a1:  dup
    IL_01a2:  ldc.i4.0
    IL_01a3:  ldloc.0
    IL_01a4:  stelem.ref
    IL_01a5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01aa:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType,int32>(object,
                                                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_01af:  pop
    IL_01b0:  ret
  } // end of method ExpressionTrees::FieldAndPropertyAccess

  .method public hidebysig static void  Call() cil managed
  {
    // Code size       527 (0x20f)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    [mscorlib]System.String
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  ldstr      "a"
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0016:  stloc.0
    IL_0017:  ldnull
    IL_0018:  ldtoken    method void [mscorlib]System.Console::WriteLine(string)
    IL_001d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0022:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0027:  ldc.i4.1
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002d:  dup
    IL_002e:  ldc.i4.0
    IL_002f:  ldloc.0
    IL_0030:  stelem.ref
    IL_0031:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0036:  ldc.i4.1
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003c:  dup
    IL_003d:  ldc.i4.0
    IL_003e:  ldloc.0
    IL_003f:  stelem.ref
    IL_0040:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Action`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                       class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0045:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<string>(object,
                                                                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Action`1<!!0>>)
    IL_004a:  pop
    IL_004b:  ldsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_0'
    IL_0050:  dup
    IL_0051:  brtrue.s   IL_006a

    IL_0053:  pop
    IL_0054:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0059:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_0'(string)
    IL_005f:  newobj     instance void class [mscorlib]System.Func`2<string,string>::.ctor(object,
                                                                                           native int)
    IL_0064:  dup
    IL_0065:  stsfld     class [mscorlib]System.Func`2<string,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_0'
    IL_006a:  ldtoken    [mscorlib]System.String
    IL_006f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0074:  ldstr      "a"
    IL_0079:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_007e:  stloc.0
    IL_007f:  ldloc.0
    IL_0080:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0085:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008f:  ldc.i4.0
    IL_0090:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0095:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009a:  ldc.i4.1
    IL_009b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a0:  dup
    IL_00a1:  ldc.i4.0
    IL_00a2:  ldloc.0
    IL_00a3:  stelem.ref
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,string>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ae:  nop
    IL_00af:  ldsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_1'
    IL_00b4:  dup
    IL_00b5:  brtrue.s   IL_00ce

    IL_00b7:  pop
    IL_00b8:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00bd:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_1'(int32)
    IL_00c3:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_00c8:  dup
    IL_00c9:  stsfld     class [mscorlib]System.Func`2<int32,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_1'
    IL_00ce:  ldtoken    [mscorlib]System.Int32
    IL_00d3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d8:  ldstr      "a"
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e2:  stloc.0
    IL_00e3:  ldloc.0
    IL_00e4:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_00e9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ee:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00f3:  ldc.i4.0
    IL_00f4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fe:  ldc.i4.1
    IL_00ff:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0104:  dup
    IL_0105:  ldc.i4.0
    IL_0106:  ldloc.0
    IL_0107:  stelem.ref
    IL_0108:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                           class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<int32,string>>(!!0,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0112:  nop
    IL_0113:  ldsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_2'
    IL_0118:  dup
    IL_0119:  brtrue.s   IL_0132

    IL_011b:  pop
    IL_011c:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0121:  ldftn      instance char[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_2'(string)
    IL_0127:  newobj     instance void class [mscorlib]System.Func`2<string,char[]>::.ctor(object,
                                                                                           native int)
    IL_012c:  dup
    IL_012d:  stsfld     class [mscorlib]System.Func`2<string,char[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_2'
    IL_0132:  ldtoken    [mscorlib]System.String
    IL_0137:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013c:  ldstr      "a"
    IL_0141:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0146:  stloc.0
    IL_0147:  ldnull
    IL_0148:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<char>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_014d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0152:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0157:  ldc.i4.1
    IL_0158:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015d:  dup
    IL_015e:  ldc.i4.0
    IL_015f:  ldloc.0
    IL_0160:  stelem.ref
    IL_0161:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0166:  ldc.i4.1
    IL_0167:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016c:  dup
    IL_016d:  ldc.i4.0
    IL_016e:  ldloc.0
    IL_016f:  stelem.ref
    IL_0170:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,char[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0175:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`2<string,char[]>>(!!0,
                                                                                                                                                class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_017a:  nop
    IL_017b:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_3'
    IL_0180:  dup
    IL_0181:  brtrue.s   IL_019a

    IL_0183:  pop
    IL_0184:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_0189:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Call>b__78_3'()
    IL_018f:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_0194:  dup
    IL_0195:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__78_3'
    IL_019a:  ldc.i4.s   97
    IL_019c:  box        [mscorlib]System.Char
    IL_01a1:  ldtoken    [mscorlib]System.Char
    IL_01a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ab:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b0:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_01b5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01ba:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01bf:  ldc.i4.1
    IL_01c0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01c5:  dup
    IL_01c6:  ldc.i4.0
    IL_01c7:  ldc.i4.s   98
    IL_01c9:  box        [mscorlib]System.Char
    IL_01ce:  ldtoken    [mscorlib]System.Char
    IL_01d3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01dd:  stelem.ref
    IL_01de:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01e3:  ldc.i4.0
    IL_01e4:  box        [mscorlib]System.Int32
    IL_01e9:  ldtoken    [mscorlib]System.Int32
    IL_01ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_01fd:  ldc.i4.0
    IL_01fe:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0203:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0208:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_020d:  nop
    IL_020e:  ret
  } // end of method ExpressionTrees::Call

  .method public hidebysig static void  Quote() cil managed
  {
    // Code size       200 (0xc8)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__79_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<Quote>b__79_0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<bool>::.ctor(object,
                                                                                  native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__79_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldstr      "n"
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0034:  stloc.0
    IL_0035:  ldtoken    [mscorlib]System.String
    IL_003a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003f:  ldstr      "s"
    IL_0044:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0049:  stloc.1
    IL_004a:  ldloc.1
    IL_004b:  ldloc.0
    IL_004c:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_006b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0070:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0075:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_007a:  ldc.i4.2
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0080:  dup
    IL_0081:  ldc.i4.0
    IL_0082:  ldloc.0
    IL_0083:  stelem.ref
    IL_0084:  dup
    IL_0085:  ldc.i4.1
    IL_0086:  ldloc.1
    IL_0087:  stelem.ref
    IL_0088:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0092:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_0097:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a1:  ldnull
    IL_00a2:  ldtoken    [mscorlib]System.Object
    IL_00a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00b6:  ldc.i4.0
    IL_00b7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00bc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<bool>>(!!0,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00c6:  nop
    IL_00c7:  ret
  } // end of method ExpressionTrees::Quote

  .method public hidebysig static void  ArrayInitializer() cil managed
  {
    // Code size       611 (0x263)
    .maxstack  11
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_0'
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  ldc.i4.3
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  dup
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.i4.1
    IL_0033:  box        [mscorlib]System.Int32
    IL_0038:  ldtoken    [mscorlib]System.Int32
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0047:  stelem.ref
    IL_0048:  dup
    IL_0049:  ldc.i4.1
    IL_004a:  ldc.i4.2
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  dup
    IL_0061:  ldc.i4.2
    IL_0062:  ldc.i4.3
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  stelem.ref
    IL_0078:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_007d:  ldc.i4.0
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0083:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0088:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_008d:  nop
    IL_008e:  ldsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_1'
    IL_0093:  dup
    IL_0094:  brtrue.s   IL_00ad

    IL_0096:  pop
    IL_0097:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_009c:  ldftn      instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_1'()
    IL_00a2:  newobj     instance void class [mscorlib]System.Func`1<int32[]>::.ctor(object,
                                                                                     native int)
    IL_00a7:  dup
    IL_00a8:  stsfld     class [mscorlib]System.Func`1<int32[]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_1'
    IL_00ad:  ldtoken    [mscorlib]System.Int32
    IL_00b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b7:  ldc.i4.1
    IL_00b8:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bd:  dup
    IL_00be:  ldc.i4.0
    IL_00bf:  ldc.i4.3
    IL_00c0:  box        [mscorlib]System.Int32
    IL_00c5:  ldtoken    [mscorlib]System.Int32
    IL_00ca:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d4:  stelem.ref
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00da:  ldc.i4.0
    IL_00db:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[]>>(!!0,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00ea:  nop
    IL_00eb:  ldsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_2'
    IL_00f0:  dup
    IL_00f1:  brtrue.s   IL_010a

    IL_00f3:  pop
    IL_00f4:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_00f9:  ldftn      instance int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_2'()
    IL_00ff:  newobj     instance void class [mscorlib]System.Func`1<int32[0...,0...]>::.ctor(object,
                                                                                              native int)
    IL_0104:  dup
    IL_0105:  stsfld     class [mscorlib]System.Func`1<int32[0...,0...]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_2'
    IL_010a:  ldtoken    [mscorlib]System.Int32
    IL_010f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0114:  ldc.i4.2
    IL_0115:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_011a:  dup
    IL_011b:  ldc.i4.0
    IL_011c:  ldc.i4.3
    IL_011d:  box        [mscorlib]System.Int32
    IL_0122:  ldtoken    [mscorlib]System.Int32
    IL_0127:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0131:  stelem.ref
    IL_0132:  dup
    IL_0133:  ldc.i4.1
    IL_0134:  ldc.i4.5
    IL_0135:  box        [mscorlib]System.Int32
    IL_013a:  ldtoken    [mscorlib]System.Int32
    IL_013f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0144:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0149:  stelem.ref
    IL_014a:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_014f:  ldc.i4.0
    IL_0150:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0155:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[0...,0...]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_015a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[0...,0...]>>(!!0,
                                                                                                                                                   class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_015f:  nop
    IL_0160:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_3'
    IL_0165:  dup
    IL_0166:  brtrue.s   IL_017f

    IL_0168:  pop
    IL_0169:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_016e:  ldftn      instance int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_3'()
    IL_0174:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_0179:  dup
    IL_017a:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_3'
    IL_017f:  ldtoken    int32[]
    IL_0184:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0189:  ldc.i4.1
    IL_018a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_018f:  dup
    IL_0190:  ldc.i4.0
    IL_0191:  ldc.i4.3
    IL_0192:  box        [mscorlib]System.Int32
    IL_0197:  ldtoken    [mscorlib]System.Int32
    IL_019c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a6:  stelem.ref
    IL_01a7:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ac:  ldc.i4.0
    IL_01ad:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01b2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b7:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_01bc:  nop
    IL_01bd:  ldsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_4'
    IL_01c2:  dup
    IL_01c3:  brtrue.s   IL_01dc

    IL_01c5:  pop
    IL_01c6:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_01cb:  ldftn      instance int32[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<ArrayInitializer>b__80_4'()
    IL_01d1:  newobj     instance void class [mscorlib]System.Func`1<int32[][]>::.ctor(object,
                                                                                       native int)
    IL_01d6:  dup
    IL_01d7:  stsfld     class [mscorlib]System.Func`1<int32[][]> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__80_4'
    IL_01dc:  ldtoken    int32[]
    IL_01e1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e6:  ldc.i4.1
    IL_01e7:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01ec:  dup
    IL_01ed:  ldc.i4.0
    IL_01ee:  ldtoken    [mscorlib]System.Int32
    IL_01f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f8:  ldc.i4.3
    IL_01f9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01fe:  dup
    IL_01ff:  ldc.i4.0
    IL_0200:  ldc.i4.1
    IL_0201:  box        [mscorlib]System.Int32
    IL_0206:  ldtoken    [mscorlib]System.Int32
    IL_020b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0210:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0215:  stelem.ref
    IL_0216:  dup
    IL_0217:  ldc.i4.1
    IL_0218:  ldc.i4.2
    IL_0219:  box        [mscorlib]System.Int32
    IL_021e:  ldtoken    [mscorlib]System.Int32
    IL_0223:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0228:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_022d:  stelem.ref
    IL_022e:  dup
    IL_022f:  ldc.i4.2
    IL_0230:  ldc.i4.3
    IL_0231:  box        [mscorlib]System.Int32
    IL_0236:  ldtoken    [mscorlib]System.Int32
    IL_023b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0240:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0245:  stelem.ref
    IL_0246:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_024b:  stelem.ref
    IL_024c:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0251:  ldc.i4.0
    IL_0252:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0257:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32[][]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_025c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<int32[][]>>(!!0,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_0261:  nop
    IL_0262:  ret
  } // end of method ExpressionTrees::ArrayInitializer

  .method public hidebysig static void  AnonymousTypes() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__81_0'
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0020

    IL_0009:  pop
    IL_000a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9'
    IL_000f:  ldftn      instance object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<AnonymousTypes>b__81_0'()
    IL_0015:  newobj     instance void class [mscorlib]System.Func`1<object>::.ctor(object,
                                                                                    native int)
    IL_001a:  dup
    IL_001b:  stsfld     class [mscorlib]System.Func`1<object> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees/'<>c'::'<>9__81_0'
    IL_0020:  ldtoken    method instance void class '<>f__AnonymousType2`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_0025:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_002a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0034:  ldc.i4.2
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003a:  dup
    IL_003b:  ldc.i4.0
    IL_003c:  ldc.i4.5
    IL_003d:  box        [mscorlib]System.Int32
    IL_0042:  ldtoken    [mscorlib]System.Int32
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  stelem.ref
    IL_0052:  dup
    IL_0053:  ldc.i4.1
    IL_0054:  ldstr      "Test"
    IL_0059:  ldtoken    [mscorlib]System.String
    IL_005e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0068:  stelem.ref
    IL_0069:  ldc.i4.2
    IL_006a:  newarr     [mscorlib]System.Reflection.MemberInfo
    IL_006f:  dup
    IL_0070:  ldc.i4.0
    IL_0071:  ldtoken    method instance !0 class '<>f__AnonymousType2`2'<int32,string>::get_A()
    IL_0076:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_007b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0080:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0085:  stelem.ref
    IL_0086:  dup
    IL_0087:  ldc.i4.1
    IL_0088:  ldtoken    method instance !1 class '<>f__AnonymousType2`2'<int32,string>::get_B()
    IL_008d:  ldtoken    class '<>f__AnonymousType2`2'<int32,string>
    IL_0092:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0097:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009c:  stelem.ref
    IL_009d:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00a2:  ldc.i4.0
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<object>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ad:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::Test<class [mscorlib]System.Func`1<object>>(!!0,
                                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<!!0>)
    IL_00b2:  nop
    IL_00b3:  ret
  } // end of method ExpressionTrees::AnonymousTypes

  .method public hidebysig static void  ObjectInit() cil managed
  {
    // Code size       129 (0x81)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType
    IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000c:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0011:  ldc.i4.2
    IL_0012:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0017:  dup
    IL_0018:  ldc.i4.0
    IL_0019:  ldtoken    method instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::set_Property(int32)
    IL_001e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0023:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0028:  ldc.i4.4
    IL_0029:  box        [mscorlib]System.Int32
    IL_002e:  ldtoken    [mscorlib]System.Int32
    IL_0033:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0038:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0042:  stelem.ref
    IL_0043:  dup
    IL_0044:  ldc.i4.1
    IL_0045:  ldtoken    field int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_004a:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004f:  ldc.i4.3
    IL_0050:  box        [mscorlib]System.Int32
    IL_0055:  ldtoken    [mscorlib]System.Int32
    IL_005a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0069:  stelem.ref
    IL_006a:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_006f:  ldc.i4.0
    IL_0070:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0075:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                             class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExpressionTrees::ToCode<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType>(object,
                                                                                                                                                                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007f:  pop
    IL_0080:  ret
  } // end of method ExpressionTrees::ObjectInit

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MyClass::.ctor()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method MyClass::op_Addition

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private int32 '<Property>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname static 
          int32  get_StaticReadonlyProperty() cil managed
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
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
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::ReadonlyField
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.3
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleType::Field
    IL_000e:  ldarg.0
    IL_000f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0014:  nop
    IL_0015:  ret
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
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ret
  } // end of method SimpleTypeWithCtor::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithCtor

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ret
  } // end of method SimpleTypeWithMultipleCtors::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 i) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ret
  } // end of method SimpleTypeWithMultipleCtors::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.SimpleTypeWithMultipleCtors

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method GenericClassWithCtor`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithCtor`1

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ret
  } // end of method GenericClassWithMultipleCtors`1::.ctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(int32 x) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ret
  } // end of method GenericClassWithMultipleCtors`1::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClassWithMultipleCtors`1

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.GenericClass`1<T>
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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

  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' E429CCA3F703A39CC5954A6572FEC9086135B34E at I_0000C034
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_0000C034 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
