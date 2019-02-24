



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly MultidimensionalArray
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module MultidimensionalArray.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray
       extends [mscorlib]System.Object
{
  .class auto ansi nested assembly beforefieldinit Generic`2<.ctor T,S>
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field private !T[0...,0...] a
    .field private !S[][0...,0...] b
    .method public hidebysig specialname 
            instance !T  get_Item(int32 i,
                                  int32 j) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  3
      .locals init (!T V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_0007:  ldarg.1
      IL_0008:  ldarg.2
      IL_0009:  call       instance !T !T[0...,0...]::Get(int32,
                                                          int32)
      IL_000e:  stloc.0
      IL_000f:  br.s       IL_0011

      IL_0011:  ldloc.0
      IL_0012:  ret
    } // end of method Generic`2::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    int32 j,
                                    !T 'value') cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_0007:  ldarg.1
      IL_0008:  ldarg.2
      IL_0009:  ldarg.3
      IL_000a:  call       instance void !T[0...,0...]::Set(int32,
                                                            int32,
                                                            !T)
      IL_000f:  ret
    } // end of method Generic`2::set_Item

    .method public hidebysig instance void 
            TestB(!S x,
                  !S& y) cil managed
    {
      // Code size       95 (0x5f)
      .maxstack  4
      .locals init (!S V_0)
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_0007:  ldc.i4.5
      IL_0008:  ldc.i4.3
      IL_0009:  ldc.i4.s   10
      IL_000b:  newarr     !S
      IL_0010:  call       instance void !S[][0...,0...]::Set(int32,
                                                              int32,
                                                              !S[])
      IL_0015:  ldarg.0
      IL_0016:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_001b:  ldc.i4.5
      IL_001c:  ldc.i4.3
      IL_001d:  call       instance !S[] !S[][0...,0...]::Get(int32,
                                                              int32)
      IL_0022:  ldc.i4.0
      IL_0023:  ldloca.s   V_0
      IL_0025:  initobj    !S
      IL_002b:  ldloc.0
      IL_002c:  stelem     !S
      IL_0031:  ldarg.0
      IL_0032:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_0037:  ldc.i4.5
      IL_0038:  ldc.i4.3
      IL_0039:  call       instance !S[] !S[][0...,0...]::Get(int32,
                                                              int32)
      IL_003e:  ldc.i4.1
      IL_003f:  ldarg.1
      IL_0040:  stelem     !S
      IL_0045:  ldarg.0
      IL_0046:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_004b:  ldc.i4.5
      IL_004c:  ldc.i4.3
      IL_004d:  call       instance !S[] !S[][0...,0...]::Get(int32,
                                                              int32)
      IL_0052:  ldc.i4.2
      IL_0053:  ldarg.2
      IL_0054:  ldobj      !S
      IL_0059:  stelem     !S
      IL_005e:  ret
    } // end of method Generic`2::TestB

    .method public hidebysig instance void 
            PassByReference(!T& arr) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_0008:  ldc.i4.s   10
      IL_000a:  ldc.i4.s   10
      IL_000c:  call       instance !T& !T[0...,0...]::Address(int32,
                                                               int32)
      IL_0011:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::PassByReference(!0&)
      IL_0016:  nop
      IL_0017:  ret
    } // end of method Generic`2::PassByReference

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       38 (0x26)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   20
      IL_0003:  ldc.i4.s   20
      IL_0005:  newobj     instance void !T[0...,0...]::.ctor(int32,
                                                              int32)
      IL_000a:  stfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_000f:  ldarg.0
      IL_0010:  ldc.i4.s   20
      IL_0012:  ldc.i4.s   20
      IL_0014:  newobj     instance void !S[][0...,0...]::.ctor(int32,
                                                                int32)
      IL_0019:  stfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_001e:  ldarg.0
      IL_001f:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0024:  nop
      IL_0025:  ret
    } // end of method Generic`2::.ctor

    .property instance !T Item(int32,
                               int32)
    {
      .get instance !T ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2::get_Item(int32,
                                                                                                               int32)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2::set_Item(int32,
                                                                                                                 int32,
                                                                                                                 !T)
    } // end of property Generic`2::Item
  } // end of class Generic`2

  .method public hidebysig instance int32[0...,0...][] 
          MakeArray() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (int32[0...,0...][] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  newarr     int32[0...,0...]
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method MultidimensionalArray::MakeArray

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method MultidimensionalArray::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
