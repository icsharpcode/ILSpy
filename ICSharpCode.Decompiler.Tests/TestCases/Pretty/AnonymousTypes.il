
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly d3qdmgj3
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module d3qdmgj3.dll
// MVID: {580C0EC8-1048-4E5F-8BCB-8AF6994DA5B8}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x017F0000


// =============== CLASS MEMBERS DECLARATION ===================

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
    // Code size       62 (0x3e)
    .maxstack  5
    .locals init (class '<>f__AnonymousType3`3'<int32,int32,int32>[] V_0,
             class '<>f__AnonymousType3`3'<int32,int32,int32>[] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     class '<>f__AnonymousType3`3'<int32,int32,int32>
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.5
    IL_000b:  ldc.i4.2
    IL_000c:  ldc.i4.m1
    IL_000d:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_0012:  stelem.ref
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.1
    IL_0015:  ldc.i4.3
    IL_0016:  ldc.i4.6
    IL_0017:  ldc.i4.s   -6
    IL_0019:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_001e:  stelem.ref
    IL_001f:  ldloc.1
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldelem.ref
    IL_0024:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002e:  nop
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldelem.ref
    IL_0032:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003c:  nop
    IL_003d:  ret
  } // end of method AnonymousTypes::SimpleArray

  .method private hidebysig instance void 
          JaggedArray() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  5
    .locals init (class '<>f__AnonymousType3`3'<int32,int32,int32>[] V_0,
             class '<>f__AnonymousType3`3'<int32,int32,int32>[][] V_1,
             class '<>f__AnonymousType3`3'<int32,int32,int32>[] V_2,
             class '<>f__AnonymousType3`3'<int32,int32,int32>[][] V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     class '<>f__AnonymousType3`3'<int32,int32,int32>
    IL_0007:  stloc.2
    IL_0008:  ldloc.2
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.5
    IL_000b:  ldc.i4.2
    IL_000c:  ldc.i4.m1
    IL_000d:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_0012:  stelem.ref
    IL_0013:  ldloc.2
    IL_0014:  ldc.i4.1
    IL_0015:  ldc.i4.3
    IL_0016:  ldc.i4.6
    IL_0017:  ldc.i4.s   -6
    IL_0019:  newobj     instance void class '<>f__AnonymousType3`3'<int32,int32,int32>::.ctor(!0,
                                                                                               !1,
                                                                                               !2)
    IL_001e:  stelem.ref
    IL_001f:  ldloc.2
    IL_0020:  stloc.0
    IL_0021:  ldc.i4.2
    IL_0022:  newarr     class '<>f__AnonymousType3`3'<int32,int32,int32>[]
    IL_0027:  stloc.3
    IL_0028:  ldloc.3
    IL_0029:  ldc.i4.0
    IL_002a:  ldloc.0
    IL_002b:  stelem.ref
    IL_002c:  ldloc.3
    IL_002d:  ldc.i4.1
    IL_002e:  ldloc.0
    IL_002f:  stelem.ref
    IL_0030:  ldloc.3
    IL_0031:  stloc.1
    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.0
    IL_0034:  ldelem.ref
    IL_0035:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003f:  nop
    IL_0040:  ldloc.0
    IL_0041:  ldc.i4.1
    IL_0042:  ldelem.ref
    IL_0043:  callvirt   instance !0 class '<>f__AnonymousType3`3'<int32,int32,int32>::get_X()
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_004d:  nop
    IL_004e:  ldloc.1
    IL_004f:  ldlen
    IL_0050:  conv.i4
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0056:  nop
    IL_0057:  ret
  } // end of method AnonymousTypes::JaggedArray

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method AnonymousTypes::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AnonymousTypes

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
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

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       29 (0x1d)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ }"
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0018:  stloc.1
    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.1
    IL_001c:  ret
  } // end of method '<>f__AnonymousType0'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       19 (0x13)
    .maxstack  2
    .locals init (class '<>f__AnonymousType0' V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     '<>f__AnonymousType0'
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldnull
    IL_0009:  ceq
    IL_000b:  ldc.i4.0
    IL_000c:  ceq
    IL_000e:  stloc.1
    IL_000f:  br.s       IL_0011

    IL_0011:  ldloc.1
    IL_0012:  ret
  } // end of method '<>f__AnonymousType0'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  1
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.1
    IL_0007:  ret
  } // end of method '<>f__AnonymousType0'::GetHashCode

} // end of class '<>f__AnonymousType0'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`1'<'<X>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType1`1'::get_X

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      " }"
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0036:  stloc.1
    IL_0037:  br.s       IL_0039

    IL_0039:  ldloc.1
    IL_003a:  ret
  } // end of method '<>f__AnonymousType1`1'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`1'<!'<X>j__TPar'> V_0,
             bool V_1)
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
    IL_0023:  nop
    IL_0024:  stloc.1
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.1
    IL_0028:  ret
  } // end of method '<>f__AnonymousType1`1'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       37 (0x25)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4     0x12721cd8
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType1`1'<!'<X>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  stloc.1
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method '<>f__AnonymousType1`1'::GetHashCode

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType1`1'::get_X()
  } // end of property '<>f__AnonymousType1`1'::X
} // end of class '<>f__AnonymousType1`1'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType2`2'<'<X>j__TPar','<Y>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Y>j__TPar' '<Y>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType2`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<Y>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType2`2'::get_Y

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
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", Y = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0037:  box        !'<Y>j__TPar'
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
  } // end of method '<>f__AnonymousType2`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0,
             bool V_1)
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
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType2`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4     0xc18f39dd
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType2`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
  } // end of method '<>f__AnonymousType2`2'::GetHashCode

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
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Y>j__TPar' '<Y>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Z>j__TPar' '<Z>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType3`3'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<Y>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType3`3'::get_Y

  .method public hidebysig specialname instance !'<Z>j__TPar' 
          get_Z() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<Z>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType3`3'::get_Z

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       119 (0x77)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", Y = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_0037:  box        !'<Y>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      ", Z = "
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  ldarg.0
    IL_0050:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_0055:  box        !'<Z>j__TPar'
    IL_005a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_005f:  pop
    IL_0060:  ldloc.0
    IL_0061:  ldstr      " }"
    IL_0066:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_006b:  pop
    IL_006c:  ldloc.0
    IL_006d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0072:  stloc.1
    IL_0073:  br.s       IL_0075

    IL_0075:  ldloc.1
    IL_0076:  ret
  } // end of method '<>f__AnonymousType3`3'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  3
    .locals init (class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'> V_0,
             bool V_1)
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
    IL_0053:  nop
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType3`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       87 (0x57)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4     0xd0c61e6a
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Y>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldc.i4     0xa5555529
    IL_003d:  ldloc.0
    IL_003e:  mul
    IL_003f:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Z>j__TPar'>::get_Default()
    IL_0044:  ldarg.0
    IL_0045:  ldfld      !2 class '<>f__AnonymousType3`3'<!'<X>j__TPar',!'<Y>j__TPar',!'<Z>j__TPar'>::'<Z>i__Field'
    IL_004a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Z>j__TPar'>::GetHashCode(!0)
    IL_004f:  add
    IL_0050:  stloc.0
    IL_0051:  ldloc.0
    IL_0052:  stloc.1
    IL_0053:  br.s       IL_0055

    IL_0055:  ldloc.1
    IL_0056:  ret
  } // end of method '<>f__AnonymousType3`3'::GetHashCode

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


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../../ICSharpCode.Decompiler.Tests/TestCases/Pretty\AnonymousTypes.res
