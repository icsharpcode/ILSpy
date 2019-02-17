



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly MultidimensionalArray.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module MultidimensionalArray.opt.dll
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
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  call       instance !T !T[0...,0...]::Get(int32,
                                                          int32)
      IL_000d:  ret
    } // end of method Generic`2::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    int32 j,
                                    !T 'value') cil managed
    {
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  ldarg.3
      IL_0009:  call       instance void !T[0...,0...]::Set(int32,
                                                            int32,
                                                            !T)
      IL_000e:  ret
    } // end of method Generic`2::set_Item

    .method public hidebysig instance void 
            TestB(!S x,
                  !S& y) cil managed
    {
      // Code size       94 (0x5e)
      .maxstack  4
      .locals init (!S V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_0006:  ldc.i4.5
      IL_0007:  ldc.i4.3
      IL_0008:  ldc.i4.s   10
      IL_000a:  newarr     !S
      IL_000f:  call       instance void !S[][0...,0...]::Set(int32,
                                                              int32,
                                                              !S[])
      IL_0014:  ldarg.0
      IL_0015:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_001a:  ldc.i4.5
      IL_001b:  ldc.i4.3
      IL_001c:  call       instance !S[] !S[][0...,0...]::Get(int32,
                                                              int32)
      IL_0021:  ldc.i4.0
      IL_0022:  ldloca.s   V_0
      IL_0024:  initobj    !S
      IL_002a:  ldloc.0
      IL_002b:  stelem     !S
      IL_0030:  ldarg.0
      IL_0031:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_0036:  ldc.i4.5
      IL_0037:  ldc.i4.3
      IL_0038:  call       instance !S[] !S[][0...,0...]::Get(int32,
                                                              int32)
      IL_003d:  ldc.i4.1
      IL_003e:  ldarg.1
      IL_003f:  stelem     !S
      IL_0044:  ldarg.0
      IL_0045:  ldfld      !1[][0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::b
      IL_004a:  ldc.i4.5
      IL_004b:  ldc.i4.3
      IL_004c:  call       instance !S[] !S[][0...,0...]::Get(int32,
                                                              int32)
      IL_0051:  ldc.i4.2
      IL_0052:  ldarg.2
      IL_0053:  ldobj      !S
      IL_0058:  stelem     !S
      IL_005d:  ret
    } // end of method Generic`2::TestB

    .method public hidebysig instance void 
            PassByReference(!T& arr) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !0[0...,0...] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::a
      IL_0007:  ldc.i4.s   10
      IL_0009:  ldc.i4.s   10
      IL_000b:  call       instance !T& !T[0...,0...]::Address(int32,
                                                               int32)
      IL_0010:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.MultidimensionalArray/Generic`2<!T,!S>::PassByReference(!0&)
      IL_0015:  ret
    } // end of method Generic`2::PassByReference

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       37 (0x25)
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
      IL_0024:  ret
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
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  newarr     int32[0...,0...]
    IL_0007:  ret
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
