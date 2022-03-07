



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly tmpA278
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
.module tmpA278.tmp
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoDecimalConstants
       extends [mscorlib]System.Object
{
  .field private static initonly valuetype [mscorlib]System.Decimal constant
  .custom instance void [mscorlib]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                  uint8,
                                                                                                  uint32,
                                                                                                  uint32,
                                                                                                  uint32) = ( 01 00 01 00 00 00 00 00 00 00 00 00 0A 00 00 00 
                                                                                                              00 00 ) 
  .method private hidebysig instance void 
          MethodWithOptionalParameter([opt] valuetype [mscorlib]System.Decimal parameter) cil managed
  {
    .param [1]
    .custom instance void [mscorlib]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                    uint8,
                                                                                                    uint32,
                                                                                                    uint32,
                                                                                                    uint32) = ( 01 00 01 00 00 00 00 00 00 00 00 00 0A 00 00 00 
                                                                                                                00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method NoDecimalConstants::MethodWithOptionalParameter

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NoDecimalConstants::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.1
    IL_0006:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_000b:  stsfld     valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoDecimalConstants::constant
    IL_0010:  ret
  } // end of method NoDecimalConstants::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoDecimalConstants


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
