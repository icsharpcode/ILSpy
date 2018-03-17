



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly WellKnownConstants
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
.module WellKnownConstants.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.WellKnownConstants
       extends [mscorlib]System.Object
{
  .field public static literal uint8 ByteMaxValue = uint8(0xFF)
  .field public static literal uint8 ByteMinValue = uint8(0x00)
  .field public static literal int8 SByteMaxValue = int8(0x7F)
  .field public static literal int8 SByteMinValue = int8(0x80)
  .field public static literal uint16 UShortMaxValue = uint16(0xFFFF)
  .field public static literal uint16 UShortMinValue = uint16(0x0000)
  .field public static literal int16 ShortMaxValue = int16(0x8000)
  .field public static literal int16 ShortMinValue = int16(0x7FFF)
  .field public static literal uint32 UIntMaxValue = uint32(0xFFFFFFFF)
  .field public static literal uint32 UIntMinValue = uint32(0x00000000)
  .field public static literal int32 IntMaxValue = int32(0x7FFFFFFF)
  .field public static literal int32 IntMinValue = int32(0x80000000)
  .field public static literal uint64 ULongMaxValue = uint64(0xFFFFFFFFFFFFFFFF)
  .field public static literal uint64 ULongMinValue = uint64(0x0)
  .field public static literal int64 LongMaxValue = int64(0x7FFFFFFFFFFFFFFF)
  .field public static literal int64 LongMinValue = int64(0x8000000000000000)
  .field public static literal float32 FloatZero = float32(0.)
  .field public static literal float32 FloatMinusZero = float32(-0.)
  .field public static literal float32 FloatNaN = float32(0xFFC00000)
  .field public static literal float32 FloatPositiveInfinity = float32(0x7F800000)
  .field public static literal float32 FloatNegativeInfinity = float32(0xFF800000)
  .field public static literal float32 FloatMaxValue = float32(3.4028235e+038)
  .field public static literal float32 FloatMinValue = float32(-3.4028235e+038)
  .field public static literal float32 FloatEpsilon = float32(1.4012985e-045)
  .field public static literal float64 DoubleZero = float64(0.)
  .field public static literal float64 DoubleMinusZero = float64(-0.)
  .field public static literal float64 DoubleNaN = float64(0xFFF8000000000000) // -1.#IND
  .field public static literal float64 DoublePositiveInfinity = float64(0x7FF0000000000000) // 1.#INF
  .field public static literal float64 DoubleNegativeInfinity = float64(0xFFF0000000000000) // -1.#INF
  .field public static literal float64 DoubleMaxValue = float64(1.7976931348623157e+308)
  .field public static literal float64 DoubleMinValue = float64(-1.7976931348623157e+308)
  .field public static literal float64 DoubleEpsilon = float64(4.9406564584124654e-324)
  .field public static initonly valuetype [mscorlib]System.Decimal DecimalMaxValue
  .custom instance void [mscorlib]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                  uint8,
                                                                                                  uint32,
                                                                                                  uint32,
                                                                                                  uint32) = ( 01 00 00 00 FF FF FF FF FF FF FF FF FF FF FF FF 
                                                                                                              00 00 ) 
  .field public static initonly valuetype [mscorlib]System.Decimal DecimalMinValue
  .custom instance void [mscorlib]System.Runtime.CompilerServices.DecimalConstantAttribute::.ctor(uint8,
                                                                                                  uint8,
                                                                                                  uint32,
                                                                                                  uint32,
                                                                                                  uint32) = ( 01 00 00 80 FF FF FF FF FF FF FF FF FF FF FF FF 
                                                                                                              00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method WellKnownConstants::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  ldc.i4.m1
    IL_0001:  ldc.i4.m1
    IL_0002:  ldc.i4.m1
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_000a:  stsfld     valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.WellKnownConstants::DecimalMaxValue
    IL_000f:  ldc.i4.m1
    IL_0010:  ldc.i4.m1
    IL_0011:  ldc.i4.m1
    IL_0012:  ldc.i4.1
    IL_0013:  ldc.i4.0
    IL_0014:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0019:  stsfld     valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.WellKnownConstants::DecimalMinValue
    IL_001e:  ret
  } // end of method WellKnownConstants::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.WellKnownConstants


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
