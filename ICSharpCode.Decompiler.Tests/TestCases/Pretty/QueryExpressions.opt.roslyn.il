
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
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
.assembly QueryExpressions
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
.module QueryExpressions.dll
// MVID: {CD7623F3-CC98-434E-A3C5-16D9445968AB}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E10000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`3'<'<Name>j__TPar','<OrderID>j__TPar','<Total>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Total>j__TPar' '<Total>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`3'::get_Name

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`3'::get_OrderID

  .method public hidebysig specialname instance !'<Total>j__TPar' 
          get_Total() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`3'::get_Total

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderID>j__TPar' OrderID,
                               !'<Total>j__TPar' Total) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType0`3'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::Equals(!0,
                                                                                                                                !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType0`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldc.i4     0xd9111ac7
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ldc.i4     0xa5555529
    IL_0038:  mul
    IL_0039:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_003e:  ldarg.0
    IL_003f:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0044:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::GetHashCode(!0)
    IL_0049:  add
    IL_004a:  ret
  } // end of method '<>f__AnonymousType0`3'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       199 (0xc7)
    .maxstack  7
    .locals init (!'<Name>j__TPar' V_0,
             !'<Name>j__TPar' V_1,
             !'<OrderID>j__TPar' V_2,
             !'<OrderID>j__TPar' V_3,
             !'<Total>j__TPar' V_4,
             !'<Total>j__TPar' V_5)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ Name = {0}, OrderID = {1}, Total = {2} }}"
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<Name>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<Name>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<Name>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<Name>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<Name>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<OrderID>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<OrderID>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<OrderID>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<OrderID>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<OrderID>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  dup
    IL_0083:  ldc.i4.2
    IL_0084:  ldarg.0
    IL_0085:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_008a:  stloc.s    V_4
    IL_008c:  ldloca.s   V_4
    IL_008e:  ldloca.s   V_5
    IL_0090:  initobj    !'<Total>j__TPar'
    IL_0096:  ldloc.s    V_5
    IL_0098:  box        !'<Total>j__TPar'
    IL_009d:  brtrue.s   IL_00b5

    IL_009f:  ldobj      !'<Total>j__TPar'
    IL_00a4:  stloc.s    V_5
    IL_00a6:  ldloca.s   V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  box        !'<Total>j__TPar'
    IL_00af:  brtrue.s   IL_00b5

    IL_00b1:  pop
    IL_00b2:  ldnull
    IL_00b3:  br.s       IL_00c0

    IL_00b5:  constrained. !'<Total>j__TPar'
    IL_00bb:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00c0:  stelem.ref
    IL_00c1:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_00c6:  ret
  } // end of method '<>f__AnonymousType0`3'::ToString

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType0`3'::get_Name()
  } // end of property '<>f__AnonymousType0`3'::Name
  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType0`3'::get_OrderID()
  } // end of property '<>f__AnonymousType0`3'::OrderID
  .property instance !'<Total>j__TPar' Total()
  {
    .get instance !'<Total>j__TPar' '<>f__AnonymousType0`3'::get_Total()
  } // end of property '<>f__AnonymousType0`3'::Total
} // end of class '<>f__AnonymousType0`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`2'<'<c>j__TPar','<o>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<c>j__TPar' '<c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<o>j__TPar' '<o>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<c>j__TPar' 
          get_c() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_c

  .method public hidebysig specialname instance !'<o>j__TPar' 
          get_o() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_o

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<c>j__TPar' c,
                               !'<o>j__TPar' o) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType1`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::Equals(!0,
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
    IL_0000:  ldc.i4     0x27a731f0
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType1`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<c>j__TPar' V_0,
             !'<c>j__TPar' V_1,
             !'<o>j__TPar' V_2,
             !'<o>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ c = {0}, o = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<c>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<c>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<c>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<c>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<c>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<o>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<o>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<o>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<o>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<o>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .property instance !'<c>j__TPar' c()
  {
    .get instance !'<c>j__TPar' '<>f__AnonymousType1`2'::get_c()
  } // end of property '<>f__AnonymousType1`2'::c
  .property instance !'<o>j__TPar' o()
  {
    .get instance !'<o>j__TPar' '<>f__AnonymousType1`2'::get_o()
  } // end of property '<>f__AnonymousType1`2'::o
} // end of class '<>f__AnonymousType1`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType2`3'<'<Name>j__TPar','<OrderID>j__TPar','<Quantity>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Quantity>j__TPar' '<Quantity>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`3'::get_Name

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`3'::get_OrderID

  .method public hidebysig specialname instance !'<Quantity>j__TPar' 
          get_Quantity() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`3'::get_Quantity

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderID>j__TPar' OrderID,
                               !'<Quantity>j__TPar' Quantity) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType2`3'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::Equals(!0,
                                                                                                                                   !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType2`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldc.i4     0x49426206
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ldc.i4     0xa5555529
    IL_0038:  mul
    IL_0039:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::get_Default()
    IL_003e:  ldarg.0
    IL_003f:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_0044:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::GetHashCode(!0)
    IL_0049:  add
    IL_004a:  ret
  } // end of method '<>f__AnonymousType2`3'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       199 (0xc7)
    .maxstack  7
    .locals init (!'<Name>j__TPar' V_0,
             !'<Name>j__TPar' V_1,
             !'<OrderID>j__TPar' V_2,
             !'<OrderID>j__TPar' V_3,
             !'<Quantity>j__TPar' V_4,
             !'<Quantity>j__TPar' V_5)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ Name = {0}, OrderID = {1}, Quantity = {2} }}"
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<Name>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<Name>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<Name>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<Name>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<Name>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<OrderID>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<OrderID>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<OrderID>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<OrderID>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<OrderID>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  dup
    IL_0083:  ldc.i4.2
    IL_0084:  ldarg.0
    IL_0085:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_008a:  stloc.s    V_4
    IL_008c:  ldloca.s   V_4
    IL_008e:  ldloca.s   V_5
    IL_0090:  initobj    !'<Quantity>j__TPar'
    IL_0096:  ldloc.s    V_5
    IL_0098:  box        !'<Quantity>j__TPar'
    IL_009d:  brtrue.s   IL_00b5

    IL_009f:  ldobj      !'<Quantity>j__TPar'
    IL_00a4:  stloc.s    V_5
    IL_00a6:  ldloca.s   V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  box        !'<Quantity>j__TPar'
    IL_00af:  brtrue.s   IL_00b5

    IL_00b1:  pop
    IL_00b2:  ldnull
    IL_00b3:  br.s       IL_00c0

    IL_00b5:  constrained. !'<Quantity>j__TPar'
    IL_00bb:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00c0:  stelem.ref
    IL_00c1:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_00c6:  ret
  } // end of method '<>f__AnonymousType2`3'::ToString

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType2`3'::get_Name()
  } // end of property '<>f__AnonymousType2`3'::Name
  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType2`3'::get_OrderID()
  } // end of property '<>f__AnonymousType2`3'::OrderID
  .property instance !'<Quantity>j__TPar'
          Quantity()
  {
    .get instance !'<Quantity>j__TPar' '<>f__AnonymousType2`3'::get_Quantity()
  } // end of property '<>f__AnonymousType2`3'::Quantity
} // end of class '<>f__AnonymousType2`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType3`2'<'<<>h__TransparentIdentifier0>j__TPar','<d>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier0>j__TPar' '<<>h__TransparentIdentifier0>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<d>j__TPar' '<d>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier0>j__TPar' 
          'get_<>h__TransparentIdentifier0'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`2'::'get_<>h__TransparentIdentifier0'

  .method public hidebysig specialname instance !'<d>j__TPar' 
          get_d() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`2'::get_d

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier0>j__TPar' '<>h__TransparentIdentifier0',
                               !'<d>j__TPar' d) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType3`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::Equals(!0,
                                                                                                                                                      !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType3`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x24cc4ce
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType3`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<<>h__TransparentIdentifier0>j__TPar' V_0,
             !'<<>h__TransparentIdentifier0>j__TPar' V_1,
             !'<d>j__TPar' V_2,
             !'<d>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ <>h__TransparentIdentifier0 = {0}, d = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<<>h__TransparentIdentifier0>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<<>h__TransparentIdentifier0>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<d>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<d>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<d>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<d>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<d>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType3`2'::ToString

  .property instance !'<<>h__TransparentIdentifier0>j__TPar'
          '<>h__TransparentIdentifier0'()
  {
    .get instance !'<<>h__TransparentIdentifier0>j__TPar' '<>f__AnonymousType3`2'::'get_<>h__TransparentIdentifier0'()
  } // end of property '<>f__AnonymousType3`2'::'<>h__TransparentIdentifier0'
  .property instance !'<d>j__TPar' d()
  {
    .get instance !'<d>j__TPar' '<>f__AnonymousType3`2'::get_d()
  } // end of property '<>f__AnonymousType3`2'::d
} // end of class '<>f__AnonymousType3`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType4`2'<'<<>h__TransparentIdentifier1>j__TPar','<x>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier1>j__TPar' '<<>h__TransparentIdentifier1>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<x>j__TPar' '<x>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier1>j__TPar' 
          'get_<>h__TransparentIdentifier1'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType4`2'::'get_<>h__TransparentIdentifier1'

  .method public hidebysig specialname instance !'<x>j__TPar' 
          get_x() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType4`2'::get_x

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier1>j__TPar' '<>h__TransparentIdentifier1',
                               !'<x>j__TPar' x) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType4`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1>j__TPar'>::Equals(!0,
                                                                                                                                                      !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType4`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x9d43c4c9
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType4`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<<>h__TransparentIdentifier1>j__TPar' V_0,
             !'<<>h__TransparentIdentifier1>j__TPar' V_1,
             !'<x>j__TPar' V_2,
             !'<x>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ <>h__TransparentIdentifier1 = {0}, x = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<<>h__TransparentIdentifier1>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<<>h__TransparentIdentifier1>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<<>h__TransparentIdentifier1>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<<>h__TransparentIdentifier1>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<<>h__TransparentIdentifier1>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<x>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<x>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<x>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<x>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<x>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType4`2'::ToString

  .property instance !'<<>h__TransparentIdentifier1>j__TPar'
          '<>h__TransparentIdentifier1'()
  {
    .get instance !'<<>h__TransparentIdentifier1>j__TPar' '<>f__AnonymousType4`2'::'get_<>h__TransparentIdentifier1'()
  } // end of property '<>f__AnonymousType4`2'::'<>h__TransparentIdentifier1'
  .property instance !'<x>j__TPar' x()
  {
    .get instance !'<x>j__TPar' '<>f__AnonymousType4`2'::get_x()
  } // end of property '<>f__AnonymousType4`2'::x
} // end of class '<>f__AnonymousType4`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType5`3'<'<Name>j__TPar','<OrderID>j__TPar','<x>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<x>j__TPar' '<x>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType5`3'::get_Name

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType5`3'::get_OrderID

  .method public hidebysig specialname instance !'<x>j__TPar' 
          get_x() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType5`3'::get_x

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderID>j__TPar' OrderID,
                               !'<x>j__TPar' x) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType5`3'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType5`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldc.i4     0x2deb7e6
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ldc.i4     0xa5555529
    IL_0038:  mul
    IL_0039:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_003e:  ldarg.0
    IL_003f:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0044:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::GetHashCode(!0)
    IL_0049:  add
    IL_004a:  ret
  } // end of method '<>f__AnonymousType5`3'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       199 (0xc7)
    .maxstack  7
    .locals init (!'<Name>j__TPar' V_0,
             !'<Name>j__TPar' V_1,
             !'<OrderID>j__TPar' V_2,
             !'<OrderID>j__TPar' V_3,
             !'<x>j__TPar' V_4,
             !'<x>j__TPar' V_5)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ Name = {0}, OrderID = {1}, x = {2} }}"
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<Name>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<Name>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<Name>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<Name>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<Name>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<OrderID>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<OrderID>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<OrderID>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<OrderID>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<OrderID>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  dup
    IL_0083:  ldc.i4.2
    IL_0084:  ldarg.0
    IL_0085:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_008a:  stloc.s    V_4
    IL_008c:  ldloca.s   V_4
    IL_008e:  ldloca.s   V_5
    IL_0090:  initobj    !'<x>j__TPar'
    IL_0096:  ldloc.s    V_5
    IL_0098:  box        !'<x>j__TPar'
    IL_009d:  brtrue.s   IL_00b5

    IL_009f:  ldobj      !'<x>j__TPar'
    IL_00a4:  stloc.s    V_5
    IL_00a6:  ldloca.s   V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  box        !'<x>j__TPar'
    IL_00af:  brtrue.s   IL_00b5

    IL_00b1:  pop
    IL_00b2:  ldnull
    IL_00b3:  br.s       IL_00c0

    IL_00b5:  constrained. !'<x>j__TPar'
    IL_00bb:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00c0:  stelem.ref
    IL_00c1:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_00c6:  ret
  } // end of method '<>f__AnonymousType5`3'::ToString

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType5`3'::get_Name()
  } // end of property '<>f__AnonymousType5`3'::Name
  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType5`3'::get_OrderID()
  } // end of property '<>f__AnonymousType5`3'::OrderID
  .property instance !'<x>j__TPar' x()
  {
    .get instance !'<x>j__TPar' '<>f__AnonymousType5`3'::get_x()
  } // end of property '<>f__AnonymousType5`3'::x
} // end of class '<>f__AnonymousType5`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType6`2'<'<o>j__TPar','<t>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<o>j__TPar' '<o>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<t>j__TPar' '<t>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<o>j__TPar' 
          get_o() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType6`2'::get_o

  .method public hidebysig specialname instance !'<t>j__TPar' 
          get_t() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType6`2'::get_t

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<o>j__TPar' o,
                               !'<t>j__TPar' t) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType6`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType6`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x9af831eb
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType6`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<o>j__TPar' V_0,
             !'<o>j__TPar' V_1,
             !'<t>j__TPar' V_2,
             !'<t>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ o = {0}, t = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<o>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<o>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<o>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<o>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<o>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<t>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<t>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<t>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<t>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<t>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType6`2'::ToString

  .property instance !'<o>j__TPar' o()
  {
    .get instance !'<o>j__TPar' '<>f__AnonymousType6`2'::get_o()
  } // end of property '<>f__AnonymousType6`2'::o
  .property instance !'<t>j__TPar' t()
  {
    .get instance !'<t>j__TPar' '<>f__AnonymousType6`2'::get_t()
  } // end of property '<>f__AnonymousType6`2'::t
} // end of class '<>f__AnonymousType6`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType7`2'<'<OrderID>j__TPar','<Total>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Total>j__TPar' '<Total>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType7`2'::get_OrderID

  .method public hidebysig specialname instance !'<Total>j__TPar' 
          get_Total() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType7`2'::get_Total

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<OrderID>j__TPar' OrderID,
                               !'<Total>j__TPar' Total) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType7`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::Equals(!0,
                                                                                                                                !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType7`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x4c4f2ab5
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType7`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<OrderID>j__TPar' V_0,
             !'<OrderID>j__TPar' V_1,
             !'<Total>j__TPar' V_2,
             !'<Total>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ OrderID = {0}, Total = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<OrderID>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<OrderID>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<OrderID>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<OrderID>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<OrderID>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<Total>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<Total>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<Total>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<Total>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<Total>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType7`2'::ToString

  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType7`2'::get_OrderID()
  } // end of property '<>f__AnonymousType7`2'::OrderID
  .property instance !'<Total>j__TPar' Total()
  {
    .get instance !'<Total>j__TPar' '<>f__AnonymousType7`2'::get_Total()
  } // end of property '<>f__AnonymousType7`2'::Total
} // end of class '<>f__AnonymousType7`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType8`2'<'<a>j__TPar','<b>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<a>j__TPar' '<a>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<b>j__TPar' '<b>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<a>j__TPar' 
          get_a() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType8`2'::get_a

  .method public hidebysig specialname instance !'<b>j__TPar' 
          get_b() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType8`2'::get_b

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<a>j__TPar' a,
                               !'<b>j__TPar' b) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType8`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType8`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0xe21831f0
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType8`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<a>j__TPar' V_0,
             !'<a>j__TPar' V_1,
             !'<b>j__TPar' V_2,
             !'<b>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ a = {0}, b = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<a>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<a>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<a>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<a>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<a>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<b>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<b>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<b>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<b>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<b>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType8`2'::ToString

  .property instance !'<a>j__TPar' a()
  {
    .get instance !'<a>j__TPar' '<>f__AnonymousType8`2'::get_a()
  } // end of property '<>f__AnonymousType8`2'::a
  .property instance !'<b>j__TPar' b()
  {
    .get instance !'<b>j__TPar' '<>f__AnonymousType8`2'::get_b()
  } // end of property '<>f__AnonymousType8`2'::b
} // end of class '<>f__AnonymousType8`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType9`2'<'<<>h__TransparentIdentifier0>j__TPar','<c>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier0>j__TPar' '<<>h__TransparentIdentifier0>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<c>j__TPar' '<c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier0>j__TPar' 
          'get_<>h__TransparentIdentifier0'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType9`2'::'get_<>h__TransparentIdentifier0'

  .method public hidebysig specialname instance !'<c>j__TPar' 
          get_c() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType9`2'::get_c

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier0>j__TPar' '<>h__TransparentIdentifier0',
                               !'<c>j__TPar' c) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType9`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::Equals(!0,
                                                                                                                                                      !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType9`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0xf5cbc4ce
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType9`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<<>h__TransparentIdentifier0>j__TPar' V_0,
             !'<<>h__TransparentIdentifier0>j__TPar' V_1,
             !'<c>j__TPar' V_2,
             !'<c>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ <>h__TransparentIdentifier0 = {0}, c = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<<>h__TransparentIdentifier0>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<<>h__TransparentIdentifier0>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<c>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<c>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<c>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<c>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<c>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType9`2'::ToString

  .property instance !'<<>h__TransparentIdentifier0>j__TPar'
          '<>h__TransparentIdentifier0'()
  {
    .get instance !'<<>h__TransparentIdentifier0>j__TPar' '<>f__AnonymousType9`2'::'get_<>h__TransparentIdentifier0'()
  } // end of property '<>f__AnonymousType9`2'::'<>h__TransparentIdentifier0'
  .property instance !'<c>j__TPar' c()
  {
    .get instance !'<c>j__TPar' '<>f__AnonymousType9`2'::get_c()
  } // end of property '<>f__AnonymousType9`2'::c
} // end of class '<>f__AnonymousType9`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType10`3'<'<Name>j__TPar','<OrderDate>j__TPar','<Total>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderDate>j__TPar' '<OrderDate>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Total>j__TPar' '<Total>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType10`3'::get_Name

  .method public hidebysig specialname instance !'<OrderDate>j__TPar' 
          get_OrderDate() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType10`3'::get_OrderDate

  .method public hidebysig specialname instance !'<Total>j__TPar' 
          get_Total() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType10`3'::get_Total

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderDate>j__TPar' OrderDate,
                               !'<Total>j__TPar' Total) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType10`3'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::Equals(!0,
                                                                                                                                    !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::Equals(!0,
                                                                                                                                !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType10`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldc.i4     0xd81da1ec
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ldc.i4     0xa5555529
    IL_0038:  mul
    IL_0039:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_003e:  ldarg.0
    IL_003f:  ldfld      !2 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0044:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::GetHashCode(!0)
    IL_0049:  add
    IL_004a:  ret
  } // end of method '<>f__AnonymousType10`3'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       199 (0xc7)
    .maxstack  7
    .locals init (!'<Name>j__TPar' V_0,
             !'<Name>j__TPar' V_1,
             !'<OrderDate>j__TPar' V_2,
             !'<OrderDate>j__TPar' V_3,
             !'<Total>j__TPar' V_4,
             !'<Total>j__TPar' V_5)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ Name = {0}, OrderDate = {1}, Total = {2} }}"
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<Name>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<Name>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<Name>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<Name>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<Name>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<OrderDate>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<OrderDate>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<OrderDate>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<OrderDate>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<OrderDate>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  dup
    IL_0083:  ldc.i4.2
    IL_0084:  ldarg.0
    IL_0085:  ldfld      !2 class '<>f__AnonymousType10`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_008a:  stloc.s    V_4
    IL_008c:  ldloca.s   V_4
    IL_008e:  ldloca.s   V_5
    IL_0090:  initobj    !'<Total>j__TPar'
    IL_0096:  ldloc.s    V_5
    IL_0098:  box        !'<Total>j__TPar'
    IL_009d:  brtrue.s   IL_00b5

    IL_009f:  ldobj      !'<Total>j__TPar'
    IL_00a4:  stloc.s    V_5
    IL_00a6:  ldloca.s   V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  box        !'<Total>j__TPar'
    IL_00af:  brtrue.s   IL_00b5

    IL_00b1:  pop
    IL_00b2:  ldnull
    IL_00b3:  br.s       IL_00c0

    IL_00b5:  constrained. !'<Total>j__TPar'
    IL_00bb:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00c0:  stelem.ref
    IL_00c1:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_00c6:  ret
  } // end of method '<>f__AnonymousType10`3'::ToString

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType10`3'::get_Name()
  } // end of property '<>f__AnonymousType10`3'::Name
  .property instance !'<OrderDate>j__TPar'
          OrderDate()
  {
    .get instance !'<OrderDate>j__TPar' '<>f__AnonymousType10`3'::get_OrderDate()
  } // end of property '<>f__AnonymousType10`3'::OrderDate
  .property instance !'<Total>j__TPar' Total()
  {
    .get instance !'<Total>j__TPar' '<>f__AnonymousType10`3'::get_Total()
  } // end of property '<>f__AnonymousType10`3'::Total
} // end of class '<>f__AnonymousType10`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType11`2'<'<c>j__TPar','<co>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<c>j__TPar' '<c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<co>j__TPar' '<co>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<c>j__TPar' 
          get_c() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType11`2'::get_c

  .method public hidebysig specialname instance !'<co>j__TPar' 
          get_co() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType11`2'::get_co

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<c>j__TPar' c,
                               !'<co>j__TPar' co) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType11`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::Equals(!0,
                                                                                                                             !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType11`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x7df4324e
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType11`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<c>j__TPar' V_0,
             !'<c>j__TPar' V_1,
             !'<co>j__TPar' V_2,
             !'<co>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ c = {0}, co = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<c>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<c>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<c>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<c>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<c>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType11`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<co>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<co>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<co>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<co>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<co>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType11`2'::ToString

  .property instance !'<c>j__TPar' c()
  {
    .get instance !'<c>j__TPar' '<>f__AnonymousType11`2'::get_c()
  } // end of property '<>f__AnonymousType11`2'::c
  .property instance !'<co>j__TPar' co()
  {
    .get instance !'<co>j__TPar' '<>f__AnonymousType11`2'::get_co()
  } // end of property '<>f__AnonymousType11`2'::co
} // end of class '<>f__AnonymousType11`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType12`2'<'<<>h__TransparentIdentifier0>j__TPar','<n>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier0>j__TPar' '<<>h__TransparentIdentifier0>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<n>j__TPar' '<n>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier0>j__TPar' 
          'get_<>h__TransparentIdentifier0'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType12`2'::'get_<>h__TransparentIdentifier0'

  .method public hidebysig specialname instance !'<n>j__TPar' 
          get_n() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType12`2'::get_n

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier0>j__TPar' '<>h__TransparentIdentifier0',
                               !'<n>j__TPar' n) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType12`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::Equals(!0,
                                                                                                                                                      !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType12`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x21eec4ce
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier0>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType12`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<<>h__TransparentIdentifier0>j__TPar' V_0,
             !'<<>h__TransparentIdentifier0>j__TPar' V_1,
             !'<n>j__TPar' V_2,
             !'<n>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ <>h__TransparentIdentifier0 = {0}, n = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier0>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<<>h__TransparentIdentifier0>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<<>h__TransparentIdentifier0>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<<>h__TransparentIdentifier0>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType12`2'<!'<<>h__TransparentIdentifier0>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<n>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<n>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<n>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<n>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<n>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType12`2'::ToString

  .property instance !'<<>h__TransparentIdentifier0>j__TPar'
          '<>h__TransparentIdentifier0'()
  {
    .get instance !'<<>h__TransparentIdentifier0>j__TPar' '<>f__AnonymousType12`2'::'get_<>h__TransparentIdentifier0'()
  } // end of property '<>f__AnonymousType12`2'::'<>h__TransparentIdentifier0'
  .property instance !'<n>j__TPar' n()
  {
    .get instance !'<n>j__TPar' '<>f__AnonymousType12`2'::get_n()
  } // end of property '<>f__AnonymousType12`2'::n
} // end of class '<>f__AnonymousType12`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType13`2'<'<Name>j__TPar','<OrderCount>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderCount>j__TPar' '<OrderCount>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType13`2'::get_Name

  .method public hidebysig specialname instance !'<OrderCount>j__TPar' 
          get_OrderCount() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType13`2'::get_OrderCount

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderCount>j__TPar' OrderCount) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType13`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::Equals(!0,
                                                                                                                                     !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType13`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0x4be01753
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType13`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<Name>j__TPar' V_0,
             !'<Name>j__TPar' V_1,
             !'<OrderCount>j__TPar' V_2,
             !'<OrderCount>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ Name = {0}, OrderCount = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<Name>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<Name>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<Name>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<Name>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<Name>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType13`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<OrderCount>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<OrderCount>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<OrderCount>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<OrderCount>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<OrderCount>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType13`2'::ToString

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType13`2'::get_Name()
  } // end of property '<>f__AnonymousType13`2'::Name
  .property instance !'<OrderCount>j__TPar'
          OrderCount()
  {
    .get instance !'<OrderCount>j__TPar' '<>f__AnonymousType13`2'::get_OrderCount()
  } // end of property '<>f__AnonymousType13`2'::OrderCount
} // end of class '<>f__AnonymousType13`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType14`2'<'<Country>j__TPar','<CustCount>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Country>j__TPar' '<Country>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<CustCount>j__TPar' '<CustCount>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname instance !'<Country>j__TPar' 
          get_Country() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType14`2'::get_Country

  .method public hidebysig specialname instance !'<CustCount>j__TPar' 
          get_CustCount() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType14`2'::get_CustCount

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Country>j__TPar' Country,
                               !'<CustCount>j__TPar' CustCount) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType14`2'::.ctor

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::Equals(!0,
                                                                                                                                    !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType14`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldc.i4     0xdf02304a
    IL_0005:  ldc.i4     0xa5555529
    IL_000a:  mul
    IL_000b:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::get_Default()
    IL_0010:  ldarg.0
    IL_0011:  ldfld      !0 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0016:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::GetHashCode(!0)
    IL_001b:  add
    IL_001c:  ldc.i4     0xa5555529
    IL_0021:  mul
    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_002d:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::GetHashCode(!0)
    IL_0032:  add
    IL_0033:  ret
  } // end of method '<>f__AnonymousType14`2'::GetHashCode

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       136 (0x88)
    .maxstack  7
    .locals init (!'<Country>j__TPar' V_0,
             !'<Country>j__TPar' V_1,
             !'<CustCount>j__TPar' V_2,
             !'<CustCount>j__TPar' V_3)
    IL_0000:  ldnull
    IL_0001:  ldstr      "{{ Country = {0}, CustCount = {1} }}"
    IL_0006:  ldc.i4.2
    IL_0007:  newarr     [mscorlib]System.Object
    IL_000c:  dup
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  ldfld      !0 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0014:  stloc.0
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldloca.s   V_1
    IL_0019:  initobj    !'<Country>j__TPar'
    IL_001f:  ldloc.1
    IL_0020:  box        !'<Country>j__TPar'
    IL_0025:  brtrue.s   IL_003b

    IL_0027:  ldobj      !'<Country>j__TPar'
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_1
    IL_002f:  ldloc.1
    IL_0030:  box        !'<Country>j__TPar'
    IL_0035:  brtrue.s   IL_003b

    IL_0037:  pop
    IL_0038:  ldnull
    IL_0039:  br.s       IL_0046

    IL_003b:  constrained. !'<Country>j__TPar'
    IL_0041:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.1
    IL_0049:  ldarg.0
    IL_004a:  ldfld      !1 class '<>f__AnonymousType14`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_2
    IL_0052:  ldloca.s   V_3
    IL_0054:  initobj    !'<CustCount>j__TPar'
    IL_005a:  ldloc.3
    IL_005b:  box        !'<CustCount>j__TPar'
    IL_0060:  brtrue.s   IL_0076

    IL_0062:  ldobj      !'<CustCount>j__TPar'
    IL_0067:  stloc.3
    IL_0068:  ldloca.s   V_3
    IL_006a:  ldloc.3
    IL_006b:  box        !'<CustCount>j__TPar'
    IL_0070:  brtrue.s   IL_0076

    IL_0072:  pop
    IL_0073:  ldnull
    IL_0074:  br.s       IL_0081

    IL_0076:  constrained. !'<CustCount>j__TPar'
    IL_007c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0081:  stelem.ref
    IL_0082:  call       string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider,
                                                                string,
                                                                object[])
    IL_0087:  ret
  } // end of method '<>f__AnonymousType14`2'::ToString

  .property instance !'<Country>j__TPar' Country()
  {
    .get instance !'<Country>j__TPar' '<>f__AnonymousType14`2'::get_Country()
  } // end of property '<>f__AnonymousType14`2'::Country
  .property instance !'<CustCount>j__TPar'
          CustCount()
  {
    .get instance !'<CustCount>j__TPar' '<>f__AnonymousType14`2'::get_CustCount()
  } // end of property '<>f__AnonymousType14`2'::CustCount
} // end of class '<>f__AnonymousType14`2'

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit Customer
         extends [mscorlib]System.Object
  {
    .field public int32 CustomerID
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> Orders
    .field public string Name
    .field public string Country
    .field public string City
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Customer::.ctor

  } // end of class Customer

  .class auto ansi nested public beforefieldinit Order
         extends [mscorlib]System.Object
  {
    .field public int32 OrderID
    .field public valuetype [mscorlib]System.DateTime OrderDate
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer Customer
    .field public int32 CustomerID
    .field public valuetype [mscorlib]System.Decimal Total
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> Details
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Order::.ctor

  } // end of class Order

  .class auto ansi nested public beforefieldinit OrderDetail
         extends [mscorlib]System.Object
  {
    .field public valuetype [mscorlib]System.Decimal UnitPrice
    .field public int32 Quantity
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method OrderDetail::.ctor

  } // end of class OrderDetail

  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' '<>9'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> '<>9__5_0'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> '<>9__5_1'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__6_0'
    .field public static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> '<>9__6_1'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__7_0'
    .field public static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__7_1'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> '<>9__7_2'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> '<>9__7_3'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__8_0'
    .field public static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__8_1'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> '<>9__8_2'
    .field public static class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> '<>9__8_3'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__9_0'
    .field public static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>9__9_1'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> '<>9__9_2'
    .field public static class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> '<>9__9_3'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> '<>9__9_4'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> '<>9__9_5'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> '<>9__10_1'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> '<>9__10_0'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> '<>9__10_2'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> '<>9__10_3'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> '<>9__11_0'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> '<>9__11_1'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> '<>9__11_2'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> '<>9__12_0'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> '<>9__12_1'
    .field public static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> '<>9__12_2'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> '<>9__13_0'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> '<>9__13_1'
    .field public static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> '<>9__13_2'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> '<>9__13_3'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> '<>9__13_4'
    .field public static class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousType13`2'<string,int32>> '<>9__13_5'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> '<>9__14_0'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> '<>9__14_1'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> '<>9__15_0'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> '<>9__15_1'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> '<>9__16_0'
    .field public static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> '<>9__17_0'
    .field public static class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousType14`2'<string,int32>> '<>9__17_1'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
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

    .method assembly hidebysig instance bool 
            '<MultipleWhere>b__5_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
      IL_0006:  call       int32 [System.Core]System.Linq.Enumerable::Count<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000b:  ldc.i4.s   10
      IL_000d:  cgt
      IL_000f:  ret
    } // end of method '<>c'::'<MultipleWhere>b__5_0'

    .method assembly hidebysig instance bool 
            '<MultipleWhere>b__5_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
      IL_0006:  ldstr      "DE"
      IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0010:  ret
    } // end of method '<>c'::'<MultipleWhere>b__5_1'

    .method assembly hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<SelectManyFollowedBySelect>b__6_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
      IL_0006:  ret
    } // end of method '<>c'::'<SelectManyFollowedBySelect>b__6_0'

    .method assembly hidebysig instance class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> 
            '<SelectManyFollowedBySelect>b__6_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                                 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_0006:  ldarg.2
      IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
      IL_000c:  ldarg.2
      IL_000d:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
      IL_0012:  newobj     instance void class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                               !1,
                                                                                                                               !2)
      IL_0017:  ret
    } // end of method '<>c'::'<SelectManyFollowedBySelect>b__6_1'

    .method assembly hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<SelectManyFollowedByOrderBy>b__7_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
      IL_0006:  ret
    } // end of method '<>c'::'<SelectManyFollowedByOrderBy>b__7_0'

    .method assembly hidebysig instance class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<SelectManyFollowedByOrderBy>b__7_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                                  class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::.ctor(!0,
                                                                                                                                                                                                                                        !1)
      IL_0007:  ret
    } // end of method '<>c'::'<SelectManyFollowedByOrderBy>b__7_1'

    .method assembly hidebysig instance valuetype [mscorlib]System.Decimal 
            '<SelectManyFollowedByOrderBy>b__7_2'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_0006:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
      IL_000b:  ret
    } // end of method '<>c'::'<SelectManyFollowedByOrderBy>b__7_2'

    .method assembly hidebysig instance class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> 
            '<SelectManyFollowedByOrderBy>b__7_3'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_c()
      IL_0006:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_000b:  ldarg.1
      IL_000c:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
      IL_0016:  ldarg.1
      IL_0017:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_001c:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
      IL_0021:  newobj     instance void class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                               !1,
                                                                                                                               !2)
      IL_0026:  ret
    } // end of method '<>c'::'<SelectManyFollowedByOrderBy>b__7_3'

    .method assembly hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<MultipleSelectManyFollowedBySelect>b__8_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
      IL_0006:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedBySelect>b__8_0'

    .method assembly hidebysig instance class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<MultipleSelectManyFollowedBySelect>b__8_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::.ctor(!0,
                                                                                                                                                                                                                                        !1)
      IL_0007:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedBySelect>b__8_1'

    .method assembly hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> 
            '<MultipleSelectManyFollowedBySelect>b__8_2'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_0006:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Details
      IL_000b:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedBySelect>b__8_2'

    .method assembly hidebysig instance class '<>f__AnonymousType2`3'<string,int32,int32> 
            '<MultipleSelectManyFollowedBySelect>b__8_3'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier0',
                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail d) cil managed
    {
      // Code size       34 (0x22)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_c()
      IL_0006:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_000b:  ldarg.1
      IL_000c:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
      IL_0016:  ldarg.2
      IL_0017:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::Quantity
      IL_001c:  newobj     instance void class '<>f__AnonymousType2`3'<string,int32,int32>::.ctor(!0,
                                                                                                  !1,
                                                                                                  !2)
      IL_0021:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedBySelect>b__8_3'

    .method assembly hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<MultipleSelectManyFollowedByLet>b__9_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
      IL_0006:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedByLet>b__9_0'

    .method assembly hidebysig instance class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
            '<MultipleSelectManyFollowedByLet>b__9_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::.ctor(!0,
                                                                                                                                                                                                                                        !1)
      IL_0007:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedByLet>b__9_1'

    .method assembly hidebysig instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> 
            '<MultipleSelectManyFollowedByLet>b__9_2'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_0006:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Details
      IL_000b:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedByLet>b__9_2'

    .method assembly hidebysig instance class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> 
            '<MultipleSelectManyFollowedByLet>b__9_3'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier0',
                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail d) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                                        !1)
      IL_0007:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedByLet>b__9_3'

    .method assembly hidebysig instance class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal> 
            '<MultipleSelectManyFollowedByLet>b__9_4'(class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> '<>h__TransparentIdentifier1') cil managed
    {
      // Code size       39 (0x27)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !1 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::get_d()
      IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::Quantity
      IL_000c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
      IL_0011:  ldarg.1
      IL_0012:  callvirt   instance !1 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::get_d()
      IL_0017:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::UnitPrice
      IL_001c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                    valuetype [mscorlib]System.Decimal)
      IL_0021:  newobj     instance void class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                                                                                                          !1)
      IL_0026:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedByLet>b__9_4'

    .method assembly hidebysig instance class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal> 
            '<MultipleSelectManyFollowedByLet>b__9_5'(class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal> '<>h__TransparentIdentifier2') cil managed
    {
      // Code size       54 (0x36)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::'get_<>h__TransparentIdentifier1'()
      IL_0006:  callvirt   instance !0 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::'get_<>h__TransparentIdentifier0'()
      IL_000b:  callvirt   instance !0 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_c()
      IL_0010:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_0015:  ldarg.1
      IL_0016:  callvirt   instance !0 class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::'get_<>h__TransparentIdentifier1'()
      IL_001b:  callvirt   instance !0 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::'get_<>h__TransparentIdentifier0'()
      IL_0020:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
      IL_0025:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
      IL_002a:  ldarg.1
      IL_002b:  callvirt   instance !1 class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::get_x()
      IL_0030:  newobj     instance void class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                               !1,
                                                                                                                               !2)
      IL_0035:  ret
    } // end of method '<>c'::'<MultipleSelectManyFollowedByLet>b__9_5'

    .method assembly hidebysig instance class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> 
            '<FromLetWhereSelect>b__10_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       49 (0x31)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.1
      IL_0002:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Details
      IL_0007:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_1'
      IL_000c:  dup
      IL_000d:  brtrue.s   IL_0026

      IL_000f:  pop
      IL_0010:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
      IL_0015:  ldftn      instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<FromLetWhereSelect>b__10_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail)
      IL_001b:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                                                                                   native int)
      IL_0020:  dup
      IL_0021:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_1'
      IL_0026:  call       valuetype [mscorlib]System.Decimal [System.Core]System.Linq.Enumerable::Sum<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,valuetype [mscorlib]System.Decimal>)
      IL_002b:  newobj     instance void class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                             !1)
      IL_0030:  ret
    } // end of method '<>c'::'<FromLetWhereSelect>b__10_0'

    .method assembly hidebysig instance valuetype [mscorlib]System.Decimal 
            '<FromLetWhereSelect>b__10_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail d) cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::UnitPrice
      IL_0006:  ldarg.1
      IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::Quantity
      IL_000c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
      IL_0011:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                    valuetype [mscorlib]System.Decimal)
      IL_0016:  ret
    } // end of method '<>c'::'<FromLetWhereSelect>b__10_1'

    .method assembly hidebysig instance bool 
            '<FromLetWhereSelect>b__10_2'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !1 class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::get_t()
      IL_0006:  ldc.i4     0x3e8
      IL_000b:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
      IL_0010:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                                valuetype [mscorlib]System.Decimal)
      IL_0015:  ret
    } // end of method '<>c'::'<FromLetWhereSelect>b__10_2'

    .method assembly hidebysig instance class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal> 
            '<FromLetWhereSelect>b__10_3'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::get_o()
      IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
      IL_000b:  ldarg.1
      IL_000c:  callvirt   instance !1 class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::get_t()
      IL_0011:  newobj     instance void class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                        !1)
      IL_0016:  ret
    } // end of method '<>c'::'<FromLetWhereSelect>b__10_3'

    .method assembly hidebysig instance class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> 
            '<MultipleLet>b__11_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer a) cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.1
      IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
      IL_0007:  newobj     instance void class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(!0,
                                                                                                                                                                    !1)
      IL_000c:  ret
    } // end of method '<>c'::'<MultipleLet>b__11_0'

    .method assembly hidebysig instance class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string> 
            '<MultipleLet>b__11_1'(class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !0 class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::get_a()
      IL_0007:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_000c:  newobj     instance void class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>::.ctor(!0,
                                                                                                                                                                                                          !1)
      IL_0011:  ret
    } // end of method '<>c'::'<MultipleLet>b__11_1'

    .method assembly hidebysig instance string 
            '<MultipleLet>b__11_2'(class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string> '<>h__TransparentIdentifier1') cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>::'get_<>h__TransparentIdentifier0'()
      IL_0006:  callvirt   instance !1 class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::get_b()
      IL_000b:  ldarg.1
      IL_000c:  callvirt   instance !1 class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>::get_c()
      IL_0011:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0016:  ret
    } // end of method '<>c'::'<MultipleLet>b__11_2'

    .method assembly hidebysig instance int32 
            '<Join>b__12_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::CustomerID
      IL_0006:  ret
    } // end of method '<>c'::'<Join>b__12_0'

    .method assembly hidebysig instance int32 
            '<Join>b__12_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::CustomerID
      IL_0006:  ret
    } // end of method '<>c'::'<Join>b__12_1'

    .method assembly hidebysig instance class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal> 
            '<Join>b__12_2'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_0006:  ldarg.2
      IL_0007:  ldfld      valuetype [mscorlib]System.DateTime ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderDate
      IL_000c:  ldarg.2
      IL_000d:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
      IL_0012:  newobj     instance void class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                              !1,
                                                                                                                                                              !2)
      IL_0017:  ret
    } // end of method '<>c'::'<Join>b__12_2'

    .method assembly hidebysig instance int32 
            '<JoinInto>b__13_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::CustomerID
      IL_0006:  ret
    } // end of method '<>c'::'<JoinInto>b__13_0'

    .method assembly hidebysig instance int32 
            '<JoinInto>b__13_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::CustomerID
      IL_0006:  ret
    } // end of method '<>c'::'<JoinInto>b__13_1'

    .method assembly hidebysig instance class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 
            '<JoinInto>b__13_2'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> co) cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  newobj     instance void class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                   !1)
      IL_0007:  ret
    } // end of method '<>c'::'<JoinInto>b__13_2'

    .method assembly hidebysig instance class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> 
            '<JoinInto>b__13_3'(class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>h__TransparentIdentifier0') cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.1
      IL_0002:  callvirt   instance !1 class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::get_co()
      IL_0007:  call       int32 [System.Core]System.Linq.Enumerable::Count<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                         !1)
      IL_0011:  ret
    } // end of method '<>c'::'<JoinInto>b__13_3'

    .method assembly hidebysig instance bool 
            '<JoinInto>b__13_4'(class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> '<>h__TransparentIdentifier1') cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !1 class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::get_n()
      IL_0006:  ldc.i4.s   10
      IL_0008:  clt
      IL_000a:  ldc.i4.0
      IL_000b:  ceq
      IL_000d:  ret
    } // end of method '<>c'::'<JoinInto>b__13_4'

    .method assembly hidebysig instance class '<>f__AnonymousType13`2'<string,int32> 
            '<JoinInto>b__13_5'(class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> '<>h__TransparentIdentifier1') cil managed
    {
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::'get_<>h__TransparentIdentifier0'()
      IL_0006:  callvirt   instance !0 class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::get_c()
      IL_000b:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_0010:  ldarg.1
      IL_0011:  callvirt   instance !1 class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::get_n()
      IL_0016:  newobj     instance void class '<>f__AnonymousType13`2'<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_001b:  ret
    } // end of method '<>c'::'<JoinInto>b__13_5'

    .method assembly hidebysig instance string 
            '<OrderBy>b__14_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Customer
      IL_0006:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_000b:  ret
    } // end of method '<>c'::'<OrderBy>b__14_0'

    .method assembly hidebysig instance valuetype [mscorlib]System.Decimal 
            '<OrderBy>b__14_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
      IL_0006:  ret
    } // end of method '<>c'::'<OrderBy>b__14_1'

    .method assembly hidebysig instance string 
            '<GroupBy>b__15_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
      IL_0006:  ret
    } // end of method '<>c'::'<GroupBy>b__15_0'

    .method assembly hidebysig instance string 
            '<GroupBy>b__15_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
      IL_0006:  ret
    } // end of method '<>c'::'<GroupBy>b__15_1'

    .method assembly hidebysig instance bool 
            '<ExplicitType>b__16_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::City
      IL_0006:  ldstr      "London"
      IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0010:  ret
    } // end of method '<>c'::'<ExplicitType>b__16_0'

    .method assembly hidebysig instance string 
            '<QueryContinuation>b__17_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
      IL_0006:  ret
    } // end of method '<>c'::'<QueryContinuation>b__17_0'

    .method assembly hidebysig instance class '<>f__AnonymousType14`2'<string,int32> 
            '<QueryContinuation>b__17_1'(class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> g) cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>::get_Key()
      IL_0006:  ldarg.1
      IL_0007:  call       int32 [System.Core]System.Linq.Enumerable::Count<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000c:  newobj     instance void class '<>f__AnonymousType14`2'<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_0011:  ret
    } // end of method '<>c'::'<QueryContinuation>b__17_1'

  } // end of class '<>c'

  .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> customers
  .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> orders
  .method public hidebysig instance object 
          MultipleWhere() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__5_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleWhere>b__5_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool>::.ctor(object,
                                                                                                                                                                native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__5_0'
    IL_0025:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,bool>)
    IL_002a:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__5_1'
    IL_002f:  dup
    IL_0030:  brtrue.s   IL_0049

    IL_0032:  pop
    IL_0033:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0038:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleWhere>b__5_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_003e:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool>::.ctor(object,
                                                                                                                                                                native int)
    IL_0043:  dup
    IL_0044:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__5_1'
    IL_0049:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,bool>)
    IL_004e:  ret
  } // end of method QueryExpressions::MultipleWhere

  .method public hidebysig instance object 
          SelectManyFollowedBySelect() cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__6_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<SelectManyFollowedBySelect>b__6_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__6_0'
    IL_0025:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__6_1'
    IL_002a:  dup
    IL_002b:  brtrue.s   IL_0044

    IL_002d:  pop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0033:  ldftn      instance class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<SelectManyFollowedBySelect>b__6_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0039:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                     native int)
    IL_003e:  dup
    IL_003f:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__6_1'
    IL_0044:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0049:  ret
  } // end of method QueryExpressions::SelectManyFollowedBySelect

  .method public hidebysig instance object 
          SelectManyFollowedByOrderBy() cil managed
  {
    // Code size       146 (0x92)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<SelectManyFollowedByOrderBy>b__7_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_0'
    IL_0025:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_1'
    IL_002a:  dup
    IL_002b:  brtrue.s   IL_0044

    IL_002d:  pop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0033:  ldftn      instance class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<SelectManyFollowedByOrderBy>b__7_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0039:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_003e:  dup
    IL_003f:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_1'
    IL_0044:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0049:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_2'
    IL_004e:  dup
    IL_004f:  brtrue.s   IL_0068

    IL_0051:  pop
    IL_0052:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0057:  ldftn      instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<SelectManyFollowedByOrderBy>b__7_2'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_005d:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                                                                                                                                                                                        native int)
    IL_0062:  dup
    IL_0063:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_2'
    IL_0068:  call       class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderByDescending<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                     class [mscorlib]System.Func`2<!!0,!!1>)
    IL_006d:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_3'
    IL_0072:  dup
    IL_0073:  brtrue.s   IL_008c

    IL_0075:  pop
    IL_0076:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_007b:  ldftn      instance class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<SelectManyFollowedByOrderBy>b__7_3'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_0081:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                    native int)
    IL_0086:  dup
    IL_0087:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__7_3'
    IL_008c:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0091:  ret
  } // end of method QueryExpressions::SelectManyFollowedByOrderBy

  .method public hidebysig instance object 
          MultipleSelectManyFollowedBySelect() cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedBySelect>b__8_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_0'
    IL_0025:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_1'
    IL_002a:  dup
    IL_002b:  brtrue.s   IL_0044

    IL_002d:  pop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0033:  ldftn      instance class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedBySelect>b__8_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0039:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_003e:  dup
    IL_003f:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_1'
    IL_0044:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0049:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_2'
    IL_004e:  dup
    IL_004f:  brtrue.s   IL_0068

    IL_0051:  pop
    IL_0052:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0057:  ldftn      instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedBySelect>b__8_2'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_005d:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                native int)
    IL_0062:  dup
    IL_0063:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_2'
    IL_0068:  ldsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_3'
    IL_006d:  dup
    IL_006e:  brtrue.s   IL_0087

    IL_0070:  pop
    IL_0071:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0076:  ldftn      instance class '<>f__AnonymousType2`3'<string,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedBySelect>b__8_3'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,
                                                                                                                                                                                                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail)
    IL_007c:  newobj     instance void class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                        native int)
    IL_0081:  dup
    IL_0082:  stsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__8_3'
    IL_0087:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_008c:  ret
  } // end of method QueryExpressions::MultipleSelectManyFollowedBySelect

  .method public hidebysig instance object 
          MultipleSelectManyFollowedByLet() cil managed
  {
    // Code size       213 (0xd5)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedByLet>b__9_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_0'
    IL_0025:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_1'
    IL_002a:  dup
    IL_002b:  brtrue.s   IL_0044

    IL_002d:  pop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0033:  ldftn      instance class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedByLet>b__9_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0039:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_003e:  dup
    IL_003f:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_1'
    IL_0044:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0049:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_2'
    IL_004e:  dup
    IL_004f:  brtrue.s   IL_0068

    IL_0051:  pop
    IL_0052:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0057:  ldftn      instance class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedByLet>b__9_2'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_005d:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                native int)
    IL_0062:  dup
    IL_0063:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_2'
    IL_0068:  ldsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_3'
    IL_006d:  dup
    IL_006e:  brtrue.s   IL_0087

    IL_0070:  pop
    IL_0071:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0076:  ldftn      instance class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedByLet>b__9_3'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail)
    IL_007c:  newobj     instance void class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_0081:  dup
    IL_0082:  stsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_3'
    IL_0087:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_008c:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_4'
    IL_0091:  dup
    IL_0092:  brtrue.s   IL_00ab

    IL_0094:  pop
    IL_0095:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_009a:  ldftn      instance class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedByLet>b__9_4'(class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>)
    IL_00a0:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               native int)
    IL_00a5:  dup
    IL_00a6:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_4'
    IL_00ab:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      class [mscorlib]System.Func`2<!!0,!!1>)
    IL_00b0:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_5'
    IL_00b5:  dup
    IL_00b6:  brtrue.s   IL_00cf

    IL_00b8:  pop
    IL_00b9:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_00be:  ldftn      instance class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleSelectManyFollowedByLet>b__9_5'(class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>)
    IL_00c4:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      native int)
    IL_00c9:  dup
    IL_00ca:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__9_5'
    IL_00cf:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             class [mscorlib]System.Func`2<!!0,!!1>)
    IL_00d4:  ret
  } // end of method QueryExpressions::MultipleSelectManyFollowedByLet

  .method public hidebysig instance object 
          FromLetWhereSelect() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<FromLetWhereSelect>b__10_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                     native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_0'
    IL_0025:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                            class [mscorlib]System.Func`2<!!0,!!1>)
    IL_002a:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_2'
    IL_002f:  dup
    IL_0030:  brtrue.s   IL_0049

    IL_0032:  pop
    IL_0033:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0038:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<FromLetWhereSelect>b__10_2'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>)
    IL_003e:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool>::.ctor(object,
                                                                                                                                                                                                                               native int)
    IL_0043:  dup
    IL_0044:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_2'
    IL_0049:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`2<!!0,bool>)
    IL_004e:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_3'
    IL_0053:  dup
    IL_0054:  brtrue.s   IL_006d

    IL_0056:  pop
    IL_0057:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_005c:  ldftn      instance class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<FromLetWhereSelect>b__10_3'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>)
    IL_0062:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                  native int)
    IL_0067:  dup
    IL_0068:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__10_3'
    IL_006d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0072:  ret
  } // end of method QueryExpressions::FromLetWhereSelect

  .method public hidebysig instance object 
          MultipleLet() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__11_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleLet>b__11_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>>::.ctor(object,
                                                                                                                                                                                                                                                                               native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__11_0'
    IL_0025:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                      class [mscorlib]System.Func`2<!!0,!!1>)
    IL_002a:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__11_1'
    IL_002f:  dup
    IL_0030:  brtrue.s   IL_0049

    IL_0032:  pop
    IL_0033:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0038:  ldftn      instance class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleLet>b__11_1'(class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>)
    IL_003e:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                           native int)
    IL_0043:  dup
    IL_0044:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__11_1'
    IL_0049:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                  class [mscorlib]System.Func`2<!!0,!!1>)
    IL_004e:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__11_2'
    IL_0053:  dup
    IL_0054:  brtrue.s   IL_006d

    IL_0056:  pop
    IL_0057:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_005c:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<MultipleLet>b__11_2'(class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>)
    IL_0062:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string>::.ctor(object,
                                                                                                                                                                                                                                              native int)
    IL_0067:  dup
    IL_0068:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__11_2'
    IL_006d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                     class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0072:  ret
  } // end of method QueryExpressions::MultipleLet

  .method public hidebysig instance object 
          Join() cil managed
  {
    // Code size       111 (0x6f)
    .maxstack  6
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_000c:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__12_0'
    IL_0011:  dup
    IL_0012:  brtrue.s   IL_002b

    IL_0014:  pop
    IL_0015:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_001a:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<Join>b__12_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0020:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32>::.ctor(object,
                                                                                                                                                                 native int)
    IL_0025:  dup
    IL_0026:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__12_0'
    IL_002b:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__12_1'
    IL_0030:  dup
    IL_0031:  brtrue.s   IL_004a

    IL_0033:  pop
    IL_0034:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0039:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<Join>b__12_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_003f:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32>::.ctor(object,
                                                                                                                                                              native int)
    IL_0044:  dup
    IL_0045:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__12_1'
    IL_004a:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__12_2'
    IL_004f:  dup
    IL_0050:  brtrue.s   IL_0069

    IL_0052:  pop
    IL_0053:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0058:  ldftn      instance class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<Join>b__12_2'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_005e:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                    native int)
    IL_0063:  dup
    IL_0064:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__12_2'
    IL_0069:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!3> [System.Core]System.Linq.Enumerable::Join<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32,class '<>f__AnonymousType10`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                               class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                               class [mscorlib]System.Func`2<!!1,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                               class [mscorlib]System.Func`3<!!0,!!1,!!3>)
    IL_006e:  ret
  } // end of method QueryExpressions::Join

  .method public hidebysig instance object 
          JoinInto() cil managed
  {
    // Code size       219 (0xdb)
    .maxstack  6
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_000c:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_0'
    IL_0011:  dup
    IL_0012:  brtrue.s   IL_002b

    IL_0014:  pop
    IL_0015:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_001a:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<JoinInto>b__13_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0020:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32>::.ctor(object,
                                                                                                                                                                 native int)
    IL_0025:  dup
    IL_0026:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_0'
    IL_002b:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_1'
    IL_0030:  dup
    IL_0031:  brtrue.s   IL_004a

    IL_0033:  pop
    IL_0034:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0039:  ldftn      instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<JoinInto>b__13_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_003f:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32>::.ctor(object,
                                                                                                                                                              native int)
    IL_0044:  dup
    IL_0045:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_1'
    IL_004a:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_2'
    IL_004f:  dup
    IL_0050:  brtrue.s   IL_0069

    IL_0052:  pop
    IL_0053:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0058:  ldftn      instance class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<JoinInto>b__13_2'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                                                                               class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_005e:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   native int)
    IL_0063:  dup
    IL_0064:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_2'
    IL_0069:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!3> [System.Core]System.Linq.Enumerable::GroupJoin<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32,class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!1,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>,!!3>)
    IL_006e:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_3'
    IL_0073:  dup
    IL_0074:  brtrue.s   IL_008d

    IL_0076:  pop
    IL_0077:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_007c:  ldftn      instance class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<JoinInto>b__13_3'(class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>)
    IL_0082:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         native int)
    IL_0087:  dup
    IL_0088:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_3'
    IL_008d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0092:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_4'
    IL_0097:  dup
    IL_0098:  brtrue.s   IL_00b1

    IL_009a:  pop
    IL_009b:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_00a0:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<JoinInto>b__13_4'(class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>)
    IL_00a6:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                           native int)
    IL_00ab:  dup
    IL_00ac:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_4'
    IL_00b1:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                            class [mscorlib]System.Func`2<!!0,bool>)
    IL_00b6:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousType13`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_5'
    IL_00bb:  dup
    IL_00bc:  brtrue.s   IL_00d5

    IL_00be:  pop
    IL_00bf:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_00c4:  ldftn      instance class '<>f__AnonymousType13`2'<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<JoinInto>b__13_5'(class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>)
    IL_00ca:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousType13`2'<string,int32>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                   native int)
    IL_00cf:  dup
    IL_00d0:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousType13`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__13_5'
    IL_00d5:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType12`2'<class '<>f__AnonymousType11`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousType13`2'<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_00da:  ret
  } // end of method QueryExpressions::JoinInto

  .method public hidebysig instance object 
          OrderBy() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__14_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<OrderBy>b__14_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string>::.ctor(object,
                                                                                                                                                               native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__14_0'
    IL_0025:  call       class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                  class [mscorlib]System.Func`2<!!0,!!1>)
    IL_002a:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__14_1'
    IL_002f:  dup
    IL_0030:  brtrue.s   IL_0049

    IL_0032:  pop
    IL_0033:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0038:  ldftn      instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<OrderBy>b__14_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_003e:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                                                                           native int)
    IL_0043:  dup
    IL_0044:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__14_1'
    IL_0049:  call       class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::ThenByDescending<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>(class [System.Core]System.Linq.IOrderedEnumerable`1<!!0>,
                                                                                                                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_004e:  ret
  } // end of method QueryExpressions::OrderBy

  .method public hidebysig instance object 
          GroupBy() cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__15_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<GroupBy>b__15_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(object,
                                                                                                                                                                  native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__15_0'
    IL_0025:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__15_1'
    IL_002a:  dup
    IL_002b:  brtrue.s   IL_0044

    IL_002d:  pop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0033:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<GroupBy>b__15_1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0039:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(object,
                                                                                                                                                                  native int)
    IL_003e:  dup
    IL_003f:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__15_1'
    IL_0044:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>> [System.Core]System.Linq.Enumerable::GroupBy<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,!!2>)
    IL_0049:  ret
  } // end of method QueryExpressions::GroupBy

  .method public hidebysig instance object 
          ExplicitType() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Cast<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.IEnumerable)
    IL_000b:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__16_0'
    IL_0010:  dup
    IL_0011:  brtrue.s   IL_002a

    IL_0013:  pop
    IL_0014:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0019:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<ExplicitType>b__16_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001f:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool>::.ctor(object,
                                                                                                                                                                native int)
    IL_0024:  dup
    IL_0025:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__16_0'
    IL_002a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,bool>)
    IL_002f:  ret
  } // end of method QueryExpressions::ExplicitType

  .method public hidebysig instance object 
          QueryContinuation() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__17_0'
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0025

    IL_000e:  pop
    IL_000f:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0014:  ldftn      instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<QueryContinuation>b__17_0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(object,
                                                                                                                                                                  native int)
    IL_001f:  dup
    IL_0020:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__17_0'
    IL_0025:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.IGrouping`2<!!1,!!0>> [System.Core]System.Linq.Enumerable::GroupBy<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_002a:  ldsfld     class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousType14`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__17_1'
    IL_002f:  dup
    IL_0030:  brtrue.s   IL_0049

    IL_0032:  pop
    IL_0033:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9'
    IL_0038:  ldftn      instance class '<>f__AnonymousType14`2'<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<QueryContinuation>b__17_1'(class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>)
    IL_003e:  newobj     instance void class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousType14`2'<string,int32>>::.ctor(object,
                                                                                                                                                                                                                                                           native int)
    IL_0043:  dup
    IL_0044:  stsfld     class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousType14`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/'<>c'::'<>9__17_1'
    IL_0049:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousType14`2'<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                  class [mscorlib]System.Func`2<!!0,!!1>)
    IL_004e:  ret
  } // end of method QueryExpressions::QueryContinuation

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method QueryExpressions::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
