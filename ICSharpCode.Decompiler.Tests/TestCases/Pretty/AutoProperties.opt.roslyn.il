
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly AutoProperties
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
.module AutoProperties.dll
// MVID: {7040A0B7-F734-4352-9C50-FCEB679C1390}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x012D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties
       extends [mscorlib]System.Object
{
  .field private initonly int32 '<A>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private int32 '<B>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static initonly int32 '<C>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static int32 '<D>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance int32 
          get_A() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<A>k__BackingField'
    IL_0006:  ret
  } // end of method AutoProperties::get_A

  .method public hidebysig specialname instance int32 
          get_B() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<B>k__BackingField'
    IL_0006:  ret
  } // end of method AutoProperties::get_B

  .method public hidebysig specialname instance void 
          set_B(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<B>k__BackingField'
    IL_0007:  ret
  } // end of method AutoProperties::set_B

  .method public hidebysig specialname static 
          int32  get_C() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<C>k__BackingField'
    IL_0005:  ret
  } // end of method AutoProperties::get_C

  .method public hidebysig specialname static 
          int32  get_D() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<D>k__BackingField'
    IL_0005:  ret
  } // end of method AutoProperties::get_D

  .method public hidebysig specialname static 
          void  set_D(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<D>k__BackingField'
    IL_0006:  ret
  } // end of method AutoProperties::set_D

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<A>k__BackingField'
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.2
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<B>k__BackingField'
    IL_000e:  ldarg.0
    IL_000f:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0014:  ret
  } // end of method AutoProperties::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<C>k__BackingField'
    IL_0006:  ldc.i4.4
    IL_0007:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::'<D>k__BackingField'
    IL_000c:  ret
  } // end of method AutoProperties::.cctor

  .property instance int32 A()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::get_A()
  } // end of property AutoProperties::A
  .property instance int32 B()
  {
    .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::get_B()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::set_B(int32)
  } // end of property AutoProperties::B
  .property int32 C()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::get_C()
  } // end of property AutoProperties::C
  .property int32 D()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::get_D()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties::set_D(int32)
  } // end of property AutoProperties::D
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.AutoProperties


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
