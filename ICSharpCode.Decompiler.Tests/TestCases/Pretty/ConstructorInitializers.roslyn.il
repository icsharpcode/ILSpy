



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ConstructorInitializers
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
.module ConstructorInitializers.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstructorInitializers
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested public beforefieldinit SimpleStruct
         extends [mscorlib]System.ValueType
  {
    .field public int32 Field1
    .field public int32 Field2
  } // end of class SimpleStruct

  .class auto ansi nested public beforefieldinit UnsafeFields
         extends [mscorlib]System.Object
  {
    .field public static int32 StaticSizeOf
    .field public int32 SizeOf
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       20 (0x14)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstructorInitializers/SimpleStruct
      IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstructorInitializers/UnsafeFields::SizeOf
      IL_000c:  ldarg.0
      IL_000d:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0012:  nop
      IL_0013:  ret
    } // end of method UnsafeFields::.ctor

    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstructorInitializers/SimpleStruct
      IL_0006:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstructorInitializers/UnsafeFields::StaticSizeOf
      IL_000b:  ret
    } // end of method UnsafeFields::.cctor

  } // end of class UnsafeFields

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method ConstructorInitializers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ConstructorInitializers


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
