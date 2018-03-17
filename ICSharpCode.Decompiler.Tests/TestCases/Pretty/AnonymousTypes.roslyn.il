



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly AnonymousTypes
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
.module AnonymousTypes.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 04 5C 7B 20 7D 01 00 54 0E 04 54 79 70 65   // ...\{ }..T..Type
                                                                                                 10 3C 41 6E 6F 6E 79 6D 6F 75 73 20 54 79 70 65   // .<Anonymous Type
                                                                                                 3E )                                              // >
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  isinst     '<>f__AnonymousType0'
    IL_0006:  ldnull
    IL_0007:  cgt.un
    IL_0009:  ret
  } // end of method '<>f__AnonymousType0'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method '<>f__AnonymousType0'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      "{ }"
    IL_0005:  ret
  } // end of method '<>f__AnonymousType0'::ToString

} // end of class '<>f__AnonymousType0'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`1'<'<X>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 0C 5C 7B 20 58 20 3D 20 7B 58 7D 20 7D 01   // ...\{ X = {X} }.
                                                                                                 00 54 0E 04 54 79 70 65 10 3C 41 6E 6F 6E 79 6D   // .T..Type.<Anonym
                                                                                                 6F 75 73 20 54 79 70 65 3E )                      // ous Type>
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`1'::get_X

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_000d:  ret
  } // end of method '<>f__AnonymousType1`1'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       36 (0x24)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`1'<!'<X>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`1'<!'<X>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0022

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  br.s       IL_0023

    IL_0022:  ldc.i4.0
    IL_0023:  ret
  } // end of method '<>f__AnonymousType1`1'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  ldc.i4     0x1df2dd8e
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ret
  } // end of method '<>f__AnonymousType1`1'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       77 (0x4d)
    .maxstack  7
    .locals init (!'<X>j__TPar' V_0,
             !'<X>j__TPar' V_1)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ X = {0} }}"
    IL_0006:  ldc.i4.1
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
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
    IL_0047:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_004c:  ret
  } // end of method '<>f__AnonymousType1`1'::ToString

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType1`1'::get_X()
  } // end of property '<>f__AnonymousType1`1'::X
} // end of class '<>f__AnonymousType1`1'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType2`2'<'<X>j__TPar','<Y>j__TPar'>
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
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`2'::get_Y

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
    IL_0008:  stfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType2`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       60 (0x3c)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
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
    IL_0000:  ldc.i4     0x60414d69
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType2`2'::GetHashCode

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
    IL_000f:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
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
    IL_004a:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
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
  } // end of method '<>f__AnonymousType2`2'::ToString

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType2`2'::get_X()
  } // end of property '<>f__AnonymousType2`2'::X
  .property instance !'<Y>j__TPar' Y()
  {
    .get instance !'<Y>j__TPar' '<>f__AnonymousType2`2'::get_Y()
  } // end of property '<>f__AnonymousType2`2'::Y
} // end of class '<>f__AnonymousType2`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType3`3'<'<X>j__TPar','<Y>j__TPar','<Z>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 1E 5C 7B 20 58 20 3D 20 7B 58 7D 2C 20 59   // ...\{ X = {X}, Y
                                                                                                 20 3D 20 7B 59 7D 2C 20 5A 20 3D 20 7B 5A 7D 20   //  = {Y}, Z = {Z} 
                                                                                                 7D 01 00 54 0E 04 54 79 70 65 10 3C 41 6E 6F 6E   // }..T..Type.<Anon
                                                                                                 79 6D 6F 75 73 20 54 79 70 65 3E )                // ymous Type>
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Y>j__TPar' '<Y>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Z>j__TPar' '<Z>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`3'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`3'::get_Y

  .method public hidebysig specialname instance !'<Z>j__TPar' 
          get_Z() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`3'::get_Z

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<Y>j__TPar' Y,
                               !'<Z>j__TPar' Z) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType3`3'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       84 (0x54)
    .maxstack  3
    .locals init (class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0052

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0052

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  brfalse.s  IL_0052

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Z>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Z>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0050:  br.s       IL_0053

    IL_0052:  ldc.i4.0
    IL_0053:  ret
  } // end of method '<>f__AnonymousType3`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldc.i4     0xb4568a5d
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ldc.i4     0xa5555529
    IL_0038:  mul
    IL_0039:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Z>j__TPar'>::get_Default()
    IL_003e:  ldarg.0
    IL_003f:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_0044:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Z>j__TPar'>::GetHashCode(!0)
    IL_0049:  add
    IL_004a:  ret
  } // end of method '<>f__AnonymousType3`3'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       199 (0xc7)
    .maxstack  7
    .locals init (!'<X>j__TPar' V_0,
             !'<X>j__TPar' V_1,
             !'<Y>j__TPar' V_2,
             !'<Y>j__TPar' V_3,
             !'<Z>j__TPar' V_4,
             !'<Z>j__TPar' V_5)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ X = {0}, Y = {1}, Z = {2} }}"
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
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
    IL_004a:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
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
    IL_0082:  dup
    IL_0083:  ldc.i4.2
    IL_0084:  ldarg.0
    IL_0085:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_008a:  stloc.s    V_4
    IL_008c:  ldloca.s   V_4
    IL_008e:  ldloca.s   V_5
    IL_0090:  initobj    !'<Z>j__TPar'
    IL_0096:  ldloc.s    V_5
    IL_0098:  box        !'<Z>j__TPar'
    IL_009d:  brtrue.s   IL_00b5

    IL_009f:  ldobj      !'<Z>j__TPar'
    IL_00a4:  stloc.s    V_5
    IL_00a6:  ldloca.s   V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  box        !'<Z>j__TPar'
    IL_00af:  brtrue.s   IL_00b5

    IL_00b1:  pop
    IL_00b2:  ldnull
    IL_00b3:  br.s       IL_00c0

    IL_00b5:  constrained. !'<Z>j__TPar'
    IL_00bb:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00c0:  stelem.ref
    IL_00c1:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_00c6:  ret
  } // end of method '<>f__AnonymousType3`3'::ToString

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType3`3'::get_X()
  } // end of property '<>f__AnonymousType3`3'::X
  .property instance !'<Y>j__TPar' Y()
  {
    .get instance !'<Y>j__TPar' '<>f__AnonymousType3`3'::get_Y()
  } // end of property '<>f__AnonymousType3`3'::Y
  .property instance !'<Z>j__TPar' Z()
  {
    .get instance !'<Z>j__TPar' '<>f__AnonymousType3`3'::get_Z()
  } // end of property '<>f__AnonymousType3`3'::Z
} // end of class '<>f__AnonymousType3`3'

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.AnonymousTypes
       extends [mscorlib]System.Object
{
  .method private hidebysig instance void 
          SimpleTypes() cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (class '<>f__AnonymousType0' V_0,
             class '<>f__AnonymousType1`1'<int32> V_1,
             class '<>f__AnonymousType2`2'<int32,int32> V_2)
    IL_0000:  nop
    IL_0001:  newobj     instance void '<>f__AnonymousType0'::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.5
    IL_0008:  newobj     instance void class '<>f__AnonymousType1`1'<int32>::.ctor(!0)
    IL_000d:  stloc.1
    IL_000e:  ldc.i4.5
    IL_000f:  ldc.i4.s   10
    IL_0011:  newobj     instance void class '<>f__AnonymousType2`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_0016:  stloc.2
    IL_0017:  ldloc.0
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001d:  nop
    IL_001e:  ldloc.1
    IL_001f:  callvirt   instance !0 class '<>f__AnonymousType1`1'<int32>::get_X()
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0029:  nop
    IL_002a:  ldloc.2
    IL_002b:  callvirt   instance !1 class '<>f__AnonymousType2`2'<int32,int32>::get_Y()
    IL_0030:  ldloc.2
    IL_0031:  callvirt   instance !0 class '<>f__AnonymousType2`2'<int32,int32>::get_X()
    IL_0036:  add
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003c:  nop
    IL_003d:  ret
  } // end of method AnonymousTypes::SimpleTypes

  .method private hidebysig instance void 
          SimpleArray() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  6
    .locals init (class '<>f__AnonymousType3`3'<int32,int32,int32>[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     class '<>f__AnonymousType3`3'<int32,int32,int32>
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.5
    IL_000a:  ldc.i4.2
    IL_000b:  ldc.i4.m1
    IL_000c:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_0011:  stelem.ref
    IL_0012:  dup
    IL_0013:  ldc.i4.1
    IL_0014:  ldc.i4.3
    IL_0015:  ldc.i4.6
    IL_0016:  ldc.i4.s   -6
    IL_0018:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_001d:  stelem.ref
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.0
    IL_0021:  ldelem.ref
    IL_0022:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002c:  nop
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.1
    IL_002f:  ldelem.ref
    IL_0030:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003a:  nop
    IL_003b:  ret
  } // end of method AnonymousTypes::SimpleArray

  .method private hidebysig instance void 
          JaggedArray() cil managed
  {
    // Code size       84 (0x54)
    .maxstack  6
    .locals init (class '<>f__AnonymousType3`3'<int32,int32,int32>[] V_0,
             class '<>f__AnonymousType3`3'<int32,int32,int32>[][] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     class '<>f__AnonymousType3`3'<int32,int32,int32>
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.5
    IL_000a:  ldc.i4.2
    IL_000b:  ldc.i4.m1
    IL_000c:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_0011:  stelem.ref
    IL_0012:  dup
    IL_0013:  ldc.i4.1
    IL_0014:  ldc.i4.3
    IL_0015:  ldc.i4.6
    IL_0016:  ldc.i4.s   -6
    IL_0018:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_001d:  stelem.ref
    IL_001e:  stloc.0
    IL_001f:  ldc.i4.2
    IL_0020:  newarr     class '<>f__AnonymousType3`3'<int32,int32,int32>[]
    IL_0025:  dup
    IL_0026:  ldc.i4.0
    IL_0027:  ldloc.0
    IL_0028:  stelem.ref
    IL_0029:  dup
    IL_002a:  ldc.i4.1
    IL_002b:  ldloc.0
    IL_002c:  stelem.ref
    IL_002d:  stloc.1
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.0
    IL_0030:  ldelem.ref
    IL_0031:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003b:  nop
    IL_003c:  ldloc.0
    IL_003d:  ldc.i4.1
    IL_003e:  ldelem.ref
    IL_003f:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0049:  nop
    IL_004a:  ldloc.1
    IL_004b:  ldlen
    IL_004c:  conv.i4
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0052:  nop
    IL_0053:  ret
  } // end of method AnonymousTypes::JaggedArray

  .method private hidebysig instance void 
          AnonymousTypeOutVar() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<int32,int32> V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.1
    IL_0004:  ldc.i4.2
    IL_0005:  newobj     instance void class '<>f__AnonymousType2`2'<int32,int32>::.ctor(!0,
                                                                                         !1)
    IL_000a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.AnonymousTypes::InlineVarDecl<class '<>f__AnonymousType2`2'<int32,int32>>(!!0&,
                                                                                                                                                      !!0)
    IL_000f:  nop
    IL_0010:  ldloc.0
    IL_0011:  callvirt   instance !0 class '<>f__AnonymousType2`2'<int32,int32>::get_X()
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  nop
    IL_001c:  ret
  } // end of method AnonymousTypes::AnonymousTypeOutVar

  .method private hidebysig static void  InlineVarDecl<T>([out] !!T& v,
                                                          !!T 'init') cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  stobj      !!T
    IL_0008:  ret
  } // end of method AnonymousTypes::InlineVarDecl

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method AnonymousTypes::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AnonymousTypes


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
