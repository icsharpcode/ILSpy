
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
// MVID: {3F2B5C60-16D2-439C-A732-A45B43BBFE06}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00AC0000


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

.class public auto ansi beforefieldinit ExpressionTrees
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit XmlReaderSettings
         extends [mscorlib]System.Object
  {
    .field assembly bool CloseInput
    .field assembly bool CheckCharacters
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method XmlReaderSettings::.ctor

  } // end of class XmlReaderSettings

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
      IL_0000:  ldsfld     !0 class ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0005:  ret
    } // end of method GenericClass`1::get_StaticProperty

    .method public hidebysig specialname static 
            void  set_StaticProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  stsfld     !0 class ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::set_StaticProperty

    .method public hidebysig specialname 
            instance !X  get_InstanceProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
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
      IL_0002:  stfld      !0 class ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
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
      .get !X ExpressionTrees/GenericClass`1::get_StaticProperty()
      .set void ExpressionTrees/GenericClass`1::set_StaticProperty(!X)
    } // end of property GenericClass`1::StaticProperty
    .property instance !X InstanceProperty()
    {
      .get instance !X ExpressionTrees/GenericClass`1::get_InstanceProperty()
      .set instance void ExpressionTrees/GenericClass`1::set_InstanceProperty(!X)
    } // end of property GenericClass`1::InstanceProperty
  } // end of class GenericClass`1

  .class auto ansi nested private beforefieldinit Magic
         extends [mscorlib]System.Object
  {
    .method public hidebysig instance bool 
            TestMagic(int32 test) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldc.i4.5
      IL_0003:  cgt
      IL_0005:  stloc.0
      IL_0006:  br.s       IL_0008

      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method Magic::TestMagic

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method Magic::.ctor

  } // end of class Magic

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
    .field public static initonly class ExpressionTrees/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<int32,string> '<>9__18_0'
    .field public static class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> '<>9__34_0'
    .field public static class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> '<>9__37_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ExpressionTrees/'<>c'::.ctor()
      IL_0005:  stsfld     class ExpressionTrees/'<>c' ExpressionTrees/'<>c'::'<>9'
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
            '<MethodGroupConstant>b__34_0'(class [mscorlib]System.Func`3<object,object,bool> f) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldnull
      IL_0002:  ldnull
      IL_0003:  callvirt   instance !2 class [mscorlib]System.Func`3<object,object,bool>::Invoke(!0,
                                                                                                 !1)
      IL_0008:  ret
    } // end of method '<>c'::'<MethodGroupConstant>b__34_0'

    .method assembly hidebysig instance int32 
            '<NestedLambda>b__37_0'(class [mscorlib]System.Func`1<int32> f) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
      IL_0006:  ret
    } // end of method '<>c'::'<NestedLambda>b__37_0'

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

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass34_0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class ExpressionTrees/Magic test
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
    } // end of method '<>c__DisplayClass34_0'::.ctor

  } // end of class '<>c__DisplayClass34_0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass37_0'
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
    } // end of method '<>c__DisplayClass37_0'::.ctor

  } // end of class '<>c__DisplayClass37_0'

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
    .field public class ExpressionTrees/XmlReaderSettings s
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

  .field private int32 'field'
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
    .locals init (class ExpressionTrees/'<>c__DisplayClass7_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass7_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ExpressionTrees/'<>c__DisplayClass7_0'::a
    IL_000d:  nop
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  ldtoken    ExpressionTrees/'<>c__DisplayClass7_0'
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass7_0'::a
    IL_0028:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0032:  ldc.i4.0
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0042:  pop
    IL_0043:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       68 (0x44)
    .maxstack  3
    .locals init (class ExpressionTrees/'<>c__DisplayClass8_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass8_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ExpressionTrees/'<>c__DisplayClass8_0'::a
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  ldtoken    ExpressionTrees/'<>c__DisplayClass8_0'
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass8_0'::a
    IL_0028:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0032:  ldc.i4.0
    IL_0033:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0038:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003d:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_002b:  call       object ExpressionTrees::ToCode<bool,bool>(object,
                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0030:  pop
    IL_0031:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       120 (0x78)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass10_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass10_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ExpressionTrees/'<>c__DisplayClass10_0'::x
    IL_000d:  nop
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldc.i4.1
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  ldtoken    [mscorlib]System.Int32
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0028:  ldloc.0
    IL_0029:  ldtoken    ExpressionTrees/'<>c__DisplayClass10_0'
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass10_0'::x
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
    IL_0071:  call       object ExpressionTrees::ToCode<int32>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0093:  call       object ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_00e0:  call       object ExpressionTrees::ToCode<int32>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_011f:  call       object ExpressionTrees::ToCode<int32>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_002f:  call       object ExpressionTrees::ToCode<string>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0082:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_00a5:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0061:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0066:  pop
    IL_0067:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       191 (0xbf)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClass18_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass18_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ldc.i4.s   20
    IL_000b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_0010:  ldsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees/'<>c'::'<>9__18_0'
    IL_0015:  dup
    IL_0016:  brtrue.s   IL_002f

    IL_0018:  pop
    IL_0019:  ldsfld     class ExpressionTrees/'<>c' ExpressionTrees/'<>c'::'<>9'
    IL_001e:  ldftn      instance string ExpressionTrees/'<>c'::'<IndexerAccess>b__18_0'(int32)
    IL_0024:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0029:  dup
    IL_002a:  stsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees/'<>c'::'<>9__18_0'
    IL_002f:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0034:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ExpressionTrees/'<>c__DisplayClass18_0'::dict
    IL_0039:  call       object ExpressionTrees::X()
    IL_003e:  ldloc.0
    IL_003f:  ldtoken    ExpressionTrees/'<>c__DisplayClass18_0'
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ExpressionTrees/'<>c__DisplayClass18_0'::dict
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
    IL_00b8:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_002f:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0154:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0135:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_010c:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_008a:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0081:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0086:  pop
    IL_0087:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       407 (0x197)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClass25_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass25_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ExpressionTrees/'<>c__DisplayClass25_0'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_0019:  call       object ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  ldtoken    ExpressionTrees/'<>c__DisplayClass25_0'
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass25_0'::x
    IL_0047:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0056:  ldloc.0
    IL_0057:  ldtoken    ExpressionTrees/'<>c__DisplayClass25_0'
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass25_0'::x
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
    IL_00fc:  ldtoken    ExpressionTrees/'<>c__DisplayClass25_0'
    IL_0101:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010b:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass25_0'::i
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
    IL_0190:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0195:  pop
    IL_0196:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass26_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass26_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s   42
    IL_000a:  stfld      uint8 ExpressionTrees/'<>c__DisplayClass26_0'::z
    IL_000f:  call       object ExpressionTrees::X()
    IL_0014:  ldloc.0
    IL_0015:  ldtoken    ExpressionTrees/'<>c__DisplayClass26_0'
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldtoken    field uint8 ExpressionTrees/'<>c__DisplayClass26_0'::z
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
    IL_006c:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_004a:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004f:  pop
    IL_0050:  call       object ExpressionTrees::X()
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
    IL_00ba:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bf:  pop
    IL_00c0:  call       object ExpressionTrees::X()
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
    IL_0112:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0117:  pop
    IL_0118:  call       object ExpressionTrees::X()
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
    IL_018b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0190:  pop
    IL_0191:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       100 (0x64)
    .maxstack  4
    .locals init (valuetype [mscorlib]System.DateTime V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_005d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0062:  pop
    IL_0063:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          Test() cil managed
  {
    // Code size       589 (0x24d)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.1
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  dup
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.1
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_0038:  ldc.i4.0
    IL_0039:  box        [mscorlib]System.Int32
    IL_003e:  ldtoken    [mscorlib]System.Int32
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0062:  pop
    IL_0063:  call       object ExpressionTrees::X()
    IL_0068:  ldarg.0
    IL_0069:  ldtoken    ExpressionTrees
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  ldtoken    method instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_007d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0082:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0087:  ldc.i4.0
    IL_0088:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_008d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0092:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_0097:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a6:  ldc.i4.0
    IL_00a7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b1:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b6:  pop
    IL_00b7:  call       object ExpressionTrees::X()
    IL_00bc:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_00cb:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00d0:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00da:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00df:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00e4:  ldc.i4.0
    IL_00e5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ef:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00f4:  pop
    IL_00f5:  call       object ExpressionTrees::X()
    IL_00fa:  ldtoken    [mscorlib]System.Int32
    IL_00ff:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0104:  ldc.i4.1
    IL_0105:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_010a:  dup
    IL_010b:  ldc.i4.0
    IL_010c:  ldc.i4.2
    IL_010d:  box        [mscorlib]System.Int32
    IL_0112:  ldtoken    [mscorlib]System.Int32
    IL_0117:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0121:  stelem.ref
    IL_0122:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0127:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_012c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0131:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0136:  ldc.i4.0
    IL_0137:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_013c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0141:  ldtoken    [mscorlib]System.Int32
    IL_0146:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_014b:  ldc.i4.2
    IL_014c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0151:  dup
    IL_0152:  ldc.i4.0
    IL_0153:  ldc.i4.2
    IL_0154:  box        [mscorlib]System.Int32
    IL_0159:  ldtoken    [mscorlib]System.Int32
    IL_015e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0163:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0168:  stelem.ref
    IL_0169:  dup
    IL_016a:  ldc.i4.1
    IL_016b:  ldc.i4.3
    IL_016c:  box        [mscorlib]System.Int32
    IL_0171:  ldtoken    [mscorlib]System.Int32
    IL_0176:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0180:  stelem.ref
    IL_0181:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0186:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_018b:  ldc.i4.0
    IL_018c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0191:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0196:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_019b:  pop
    IL_019c:  call       object ExpressionTrees::X()
    IL_01a1:  ldarg.0
    IL_01a2:  ldtoken    ExpressionTrees
    IL_01a7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ac:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b1:  ldtoken    method instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_01b6:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01bb:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01c0:  ldc.i4.0
    IL_01c1:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01c6:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01cb:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_01d0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01d5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01da:  ldc.i4.1
    IL_01db:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e0:  dup
    IL_01e1:  ldc.i4.0
    IL_01e2:  ldtoken    [mscorlib]System.Object
    IL_01e7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ec:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_01f1:  stelem.ref
    IL_01f2:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f7:  ldc.i4.0
    IL_01f8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01fd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0202:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0207:  pop
    IL_0208:  call       object ExpressionTrees::X()
    IL_020d:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0212:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0217:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_021c:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_0221:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0226:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0230:  ldc.i4.0
    IL_0231:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0236:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_023b:  ldc.i4.0
    IL_023c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0241:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0246:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_024b:  pop
    IL_024c:  ret
  } // end of method ExpressionTrees::Test

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       354 (0x162)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.s   37
    IL_0004:  stfld      int32 ExpressionTrees::'field'
    IL_0009:  call       object ExpressionTrees::X()
    IL_000e:  ldarg.0
    IL_000f:  ldtoken    ExpressionTrees
    IL_0014:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0019:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001e:  ldtoken    field int32 ExpressionTrees::'field'
    IL_0023:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_002d:  ldarg.0
    IL_002e:  ldtoken    ExpressionTrees
    IL_0033:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0038:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003d:  ldtoken    method instance int32 ExpressionTrees::C()
    IL_0042:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0047:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_004c:  ldc.i4.0
    IL_004d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0052:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0057:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_005c:  ldc.i4.0
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0062:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0067:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006c:  pop
    IL_006d:  call       object ExpressionTrees::X()
    IL_0072:  ldnull
    IL_0073:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_0078:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0082:  ldc.i4.2
    IL_0083:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0088:  dup
    IL_0089:  ldc.i4.0
    IL_008a:  ldarg.0
    IL_008b:  ldtoken    ExpressionTrees
    IL_0090:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009a:  stelem.ref
    IL_009b:  dup
    IL_009c:  ldc.i4.1
    IL_009d:  ldtoken    ExpressionTrees
    IL_00a2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a7:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_00ac:  stelem.ref
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00b7:  ldc.i4.0
    IL_00b8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00bd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c2:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00c7:  pop
    IL_00c8:  call       object ExpressionTrees::X()
    IL_00cd:  ldarg.0
    IL_00ce:  ldtoken    ExpressionTrees
    IL_00d3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00dd:  ldtoken    method instance bool ExpressionTrees::MyEquals(class ExpressionTrees)
    IL_00e2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ec:  ldc.i4.1
    IL_00ed:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00f2:  dup
    IL_00f3:  ldc.i4.0
    IL_00f4:  ldarg.0
    IL_00f5:  ldtoken    ExpressionTrees
    IL_00fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0104:  stelem.ref
    IL_0105:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_010a:  ldarg.0
    IL_010b:  ldtoken    ExpressionTrees
    IL_0110:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0115:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011a:  ldtoken    method instance bool ExpressionTrees::MyEquals(class ExpressionTrees)
    IL_011f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0124:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0129:  ldc.i4.1
    IL_012a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_012f:  dup
    IL_0130:  ldc.i4.0
    IL_0131:  ldtoken    ExpressionTrees
    IL_0136:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0140:  stelem.ref
    IL_0141:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0146:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_014b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0150:  ldc.i4.0
    IL_0151:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0156:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_015b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0160:  pop
    IL_0161:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 ExpressionTrees::'field'
    IL_0007:  ldc.i4.5
    IL_0008:  add
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ExpressionTrees other) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldnull
    IL_0003:  cgt.un
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brfalse.s  IL_001b

    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  ldfld      int32 ExpressionTrees::'field'
    IL_0010:  ldarg.1
    IL_0011:  ldfld      int32 ExpressionTrees::'field'
    IL_0016:  ceq
    IL_0018:  stloc.1
    IL_0019:  br.s       IL_001f

    IL_001b:  ldc.i4.0
    IL_001c:  stloc.1
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.1
    IL_0020:  ret
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       259 (0x103)
    .maxstack  12
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_00dc:  stelem.ref
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00e2:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_00e7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00f1:  ldc.i4.0
    IL_00f2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00fc:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0101:  pop
    IL_0102:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       832 (0x340)
    .maxstack  13
    .locals init (class ExpressionTrees/'<>c__DisplayClass34_0' V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass34_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  call       object ExpressionTrees::X()
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
    IL_0133:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0138:  pop
    IL_0139:  ldloc.0
    IL_013a:  newobj     instance void ExpressionTrees/Magic::.ctor()
    IL_013f:  stfld      class ExpressionTrees/Magic ExpressionTrees/'<>c__DisplayClass34_0'::test
    IL_0144:  call       object ExpressionTrees::X()
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
    IL_01e9:  ldtoken    [mscorlib]System.Int32
    IL_01ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f3:  ldstr      "bob"
    IL_01f8:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01fd:  stloc.1
    IL_01fe:  ldloc.0
    IL_01ff:  ldtoken    ExpressionTrees/'<>c__DisplayClass34_0'
    IL_0204:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0209:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_020e:  ldtoken    field class ExpressionTrees/Magic ExpressionTrees/'<>c__DisplayClass34_0'::test
    IL_0213:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0218:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_021d:  ldtoken    method instance bool ExpressionTrees/Magic::TestMagic(int32)
    IL_0222:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0227:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_022c:  ldc.i4.1
    IL_022d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0232:  dup
    IL_0233:  ldc.i4.0
    IL_0234:  ldloc.1
    IL_0235:  stelem.ref
    IL_0236:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_023b:  ldc.i4.1
    IL_023c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0241:  dup
    IL_0242:  ldc.i4.0
    IL_0243:  ldloc.1
    IL_0244:  stelem.ref
    IL_0245:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_024a:  stelem.ref
    IL_024b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0250:  ldc.i4.0
    IL_0251:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0256:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_025b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0260:  pop
    IL_0261:  ldloc.0
    IL_0262:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c'::'<>9__34_0'
    IL_0267:  dup
    IL_0268:  brtrue.s   IL_0281

    IL_026a:  pop
    IL_026b:  ldsfld     class ExpressionTrees/'<>c' ExpressionTrees/'<>c'::'<>9'
    IL_0270:  ldftn      instance bool ExpressionTrees/'<>c'::'<MethodGroupConstant>b__34_0'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_0276:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_027b:  dup
    IL_027c:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c'::'<>9__34_0'
    IL_0281:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c__DisplayClass34_0'::sink
    IL_0286:  call       object ExpressionTrees::X()
    IL_028b:  ldloc.0
    IL_028c:  ldtoken    ExpressionTrees/'<>c__DisplayClass34_0'
    IL_0291:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0296:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_029b:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c__DisplayClass34_0'::sink
    IL_02a0:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02a5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02aa:  ldc.i4.1
    IL_02ab:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02b0:  dup
    IL_02b1:  ldc.i4.0
    IL_02b2:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02b7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02bc:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02c1:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_02c6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02cb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02d0:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_02d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02da:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02df:  ldc.i4.2
    IL_02e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02e5:  dup
    IL_02e6:  ldc.i4.0
    IL_02e7:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_02ec:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f1:  ldtoken    [mscorlib]System.Type
    IL_02f6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02fb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0300:  stelem.ref
    IL_0301:  dup
    IL_0302:  ldc.i4.1
    IL_0303:  ldnull
    IL_0304:  ldtoken    [mscorlib]System.Object
    IL_0309:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_030e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0313:  stelem.ref
    IL_0314:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0319:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_031e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0323:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0328:  stelem.ref
    IL_0329:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_032e:  ldc.i4.0
    IL_032f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0334:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0339:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_033e:  pop
    IL_033f:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_005e:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0088:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008d:  pop
    IL_008e:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       548 (0x224)
    .maxstack  12
    .locals init (class ExpressionTrees/'<>c__DisplayClass37_0' V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass37_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c'::'<>9__37_0'
    IL_000d:  dup
    IL_000e:  brtrue.s   IL_0027

    IL_0010:  pop
    IL_0011:  ldsfld     class ExpressionTrees/'<>c' ExpressionTrees/'<>c'::'<>9'
    IL_0016:  ldftn      instance int32 ExpressionTrees/'<>c'::'<NestedLambda>b__37_0'(class [mscorlib]System.Func`1<int32>)
    IL_001c:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_0021:  dup
    IL_0022:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c'::'<>9__37_0'
    IL_0027:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c__DisplayClass37_0'::'call'
    IL_002c:  call       object ExpressionTrees::X()
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    ExpressionTrees/'<>c__DisplayClass37_0'
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0041:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c__DisplayClass37_0'::'call'
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
    IL_008a:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008f:  pop
    IL_0090:  call       object ExpressionTrees::X()
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
    IL_0147:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_014c:  pop
    IL_014d:  call       object ExpressionTrees::X()
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
    IL_021d:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_007f:  call       object ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
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

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       510 (0x1fe)
    .maxstack  11
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  ldtoken    ExpressionTrees
    IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0011:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0016:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
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
    IL_007c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0081:  pop
    IL_0082:  call       object ExpressionTrees::X()
    IL_0087:  ldarg.0
    IL_0088:  ldtoken    ExpressionTrees
    IL_008d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0092:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0097:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_009c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00a1:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a6:  ldc.i4.1
    IL_00a7:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ac:  dup
    IL_00ad:  ldc.i4.0
    IL_00ae:  ldtoken    [mscorlib]System.Int32
    IL_00b3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b8:  ldstr      "x"
    IL_00bd:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00c2:  stloc.0
    IL_00c3:  ldloc.0
    IL_00c4:  ldc.i4.s   37
    IL_00c6:  box        [mscorlib]System.Int32
    IL_00cb:  ldtoken    [mscorlib]System.Int32
    IL_00d0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00da:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00df:  ldc.i4.1
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  dup
    IL_00e6:  ldc.i4.0
    IL_00e7:  ldloc.0
    IL_00e8:  stelem.ref
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ee:  stelem.ref
    IL_00ef:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00f4:  ldc.i4.0
    IL_00f5:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ff:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0104:  pop
    IL_0105:  call       object ExpressionTrees::X()
    IL_010a:  ldarg.0
    IL_010b:  ldtoken    ExpressionTrees
    IL_0110:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0115:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011a:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_011f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0124:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0129:  ldc.i4.1
    IL_012a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_012f:  dup
    IL_0130:  ldc.i4.0
    IL_0131:  ldtoken    [mscorlib]System.Int32
    IL_0136:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013b:  ldstr      "x"
    IL_0140:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0145:  stloc.0
    IL_0146:  ldc.i4.1
    IL_0147:  box        [mscorlib]System.Boolean
    IL_014c:  ldtoken    [mscorlib]System.Boolean
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015b:  ldc.i4.1
    IL_015c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0161:  dup
    IL_0162:  ldc.i4.0
    IL_0163:  ldloc.0
    IL_0164:  stelem.ref
    IL_0165:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016a:  stelem.ref
    IL_016b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0170:  ldc.i4.0
    IL_0171:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0176:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_017b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0180:  pop
    IL_0181:  call       object ExpressionTrees::X()
    IL_0186:  ldarg.0
    IL_0187:  ldtoken    ExpressionTrees
    IL_018c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0191:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0196:  ldtoken    method instance bool ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_019b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01a0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01a5:  ldc.i4.1
    IL_01a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01ab:  dup
    IL_01ac:  ldc.i4.0
    IL_01ad:  ldtoken    [mscorlib]System.Int32
    IL_01b2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b7:  ldstr      "x"
    IL_01bc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01c1:  stloc.0
    IL_01c2:  ldc.i4.1
    IL_01c3:  box        [mscorlib]System.Boolean
    IL_01c8:  ldtoken    [mscorlib]System.Boolean
    IL_01cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d7:  ldc.i4.1
    IL_01d8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01dd:  dup
    IL_01de:  ldc.i4.0
    IL_01df:  ldloc.0
    IL_01e0:  stelem.ref
    IL_01e1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01e6:  stelem.ref
    IL_01e7:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ec:  ldc.i4.0
    IL_01ed:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01f2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01f7:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01fc:  pop
    IL_01fd:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       291 (0x123)
    .maxstack  12
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_011c:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0084:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0034:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0039:  pop
    IL_003a:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       271 (0x10f)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass46_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass46_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ExpressionTrees/'<>c__DisplayClass46_0'::x
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.3
    IL_0010:  stfld      int32 ExpressionTrees/'<>c__DisplayClass46_0'::y
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s   42
    IL_0018:  stfld      uint8 ExpressionTrees/'<>c__DisplayClass46_0'::z
    IL_001d:  call       object ExpressionTrees::X()
    IL_0022:  ldloc.0
    IL_0023:  ldtoken    ExpressionTrees/'<>c__DisplayClass46_0'
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldtoken    field uint8 ExpressionTrees/'<>c__DisplayClass46_0'::z
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
    IL_007a:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_007f:  pop
    IL_0080:  call       object ExpressionTrees::X()
    IL_0085:  ldloc.0
    IL_0086:  ldtoken    ExpressionTrees/'<>c__DisplayClass46_0'
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0095:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass46_0'::y
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
    IL_00ce:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00d3:  pop
    IL_00d4:  call       object ExpressionTrees::X()
    IL_00d9:  ldloc.0
    IL_00da:  ldtoken    ExpressionTrees/'<>c__DisplayClass46_0'
    IL_00df:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e9:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass46_0'::x
    IL_00ee:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f3:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00fd:  ldc.i4.0
    IL_00fe:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0103:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0108:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_010d:  pop
    IL_010e:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       269 (0x10d)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClass47_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass47_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void ExpressionTrees/XmlReaderSettings::.ctor()
    IL_000d:  dup
    IL_000e:  ldc.i4.0
    IL_000f:  stfld      bool ExpressionTrees/XmlReaderSettings::CloseInput
    IL_0014:  dup
    IL_0015:  ldc.i4.0
    IL_0016:  stfld      bool ExpressionTrees/XmlReaderSettings::CheckCharacters
    IL_001b:  stfld      class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_0020:  call       object ExpressionTrees::X()
    IL_0025:  ldtoken    ExpressionTrees/XmlReaderSettings
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0034:  ldc.i4.2
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_003a:  dup
    IL_003b:  ldc.i4.0
    IL_003c:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CloseInput
    IL_0041:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0046:  ldloc.0
    IL_0047:  ldtoken    ExpressionTrees/'<>c__DisplayClass47_0'
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0056:  ldtoken    field class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_005b:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0065:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CloseInput
    IL_006a:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0074:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0079:  stelem.ref
    IL_007a:  dup
    IL_007b:  ldc.i4.1
    IL_007c:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CheckCharacters
    IL_0081:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0086:  ldloc.0
    IL_0087:  ldtoken    ExpressionTrees/'<>c__DisplayClass47_0'
    IL_008c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0096:  ldtoken    field class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_009b:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00a5:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CheckCharacters
    IL_00aa:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00af:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00b9:  stelem.ref
    IL_00ba:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00bf:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00c4:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00c9:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ce:  ldc.i4.1
    IL_00cf:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00d4:  dup
    IL_00d5:  ldc.i4.0
    IL_00d6:  ldloc.0
    IL_00d7:  ldtoken    ExpressionTrees/'<>c__DisplayClass47_0'
    IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e6:  ldtoken    field class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass47_0'::s
    IL_00eb:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f0:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00f5:  stelem.ref
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fb:  ldc.i4.0
    IL_00fc:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0101:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0106:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_010b:  pop
    IL_010c:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       202 (0xca)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0087:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_008c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0091:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0096:  ldc.i4.1
    IL_0097:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009c:  dup
    IL_009d:  ldc.i4.0
    IL_009e:  ldstr      "fds"
    IL_00a3:  ldtoken    [mscorlib]System.String
    IL_00a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b2:  stelem.ref
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b8:  ldc.i4.0
    IL_00b9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00be:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00c3:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00c8:  pop
    IL_00c9:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       170 (0xaa)
    .maxstack  9
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method object ExpressionTrees::ToCode<bool>(object,
                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  dup
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method object ExpressionTrees::X()
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
    IL_007e:  ldstr      ""
    IL_0083:  ldtoken    [mscorlib]System.String
    IL_0088:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0092:  stelem.ref
    IL_0093:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0098:  ldc.i4.0
    IL_0099:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_009e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a3:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00a8:  pop
    IL_00a9:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       348 (0x15c)
    .maxstack  22
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0155:  call       object ExpressionTrees::ToCode<string>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_007a:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  ldtoken    ExpressionTrees
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
    IL_0067:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_001f:  ldtoken    ExpressionTrees
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
    IL_0066:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_005e:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_007a:  call       object ExpressionTrees::ToCode<bool>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_00d2:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00d7:  pop
    IL_00d8:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       407 (0x197)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClass57_0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass57_0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ExpressionTrees/'<>c__DisplayClass57_0'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_0019:  call       object ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  ldtoken    ExpressionTrees/'<>c__DisplayClass57_0'
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0042:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass57_0'::x
    IL_0047:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0056:  ldloc.0
    IL_0057:  ldtoken    ExpressionTrees/'<>c__DisplayClass57_0'
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass57_0'::x
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
    IL_00fc:  ldtoken    ExpressionTrees/'<>c__DisplayClass57_0'
    IL_0101:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0106:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_010b:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass57_0'::i
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
    IL_0190:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0195:  pop
    IL_0196:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          StringAccessor() cil managed
  {
    // Code size       136 (0x88)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
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
    IL_0081:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0086:  pop
    IL_0087:  ret
  } // end of method ExpressionTrees::StringAccessor

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       119 (0x77)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0015:  ldtoken    field !0 class ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_001a:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_001f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0029:  ldtoken    [mscorlib]System.Double
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0038:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Type)
    IL_0047:  ldtoken    method instance !0 class ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_004c:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
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
    IL_0070:  call       object ExpressionTrees::ToCode<float64>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    field !0 class ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000c:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001b:  ldtoken    [mscorlib]System.Double
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_002a:  ldnull
    IL_002b:  ldtoken    method !0 class ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_0030:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
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
    IL_0054:  call       object ExpressionTrees::ToCode<float64>(object,
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
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool class ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000c:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
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
    IL_0031:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0036:  pop
    IL_0037:  ret
  } // end of method ExpressionTrees::InvokeGenericMethod

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

} // end of class ExpressionTrees


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
