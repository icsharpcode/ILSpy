



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

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
  .field public static literal float32 Float_One = float32(1.)
  .field public static literal float64 Double_One = float64(1.)
  .field public static literal float32 Float_Two = float32(2.)
  .field public static literal float64 Double_Two = float64(2.)
  .field public static literal float32 Float_PI = float32(3.1415927)
  .field public static literal float32 Float_HalfOfPI = float32(1.5707964)
  .field public static literal float32 Float_QuarterOfPI = float32(0.78539819)
  .field public static literal float32 Float_PITimes2 = float32(6.2831855)
  .field public static literal float32 Float_3QuartersOfPI = float32(2.3561945)
  .field public static literal float32 Float_PIDiv360 = float32(8.7266462e-003)
  .field public static literal float32 Float_PIDiv16 = float32(0.19634955)
  .field public static literal float32 Float_PIDiv32 = float32(9.8174773e-002)
  .field public static literal float32 Float_PIInverseFraction = float32(0.31830987)
  .field public static literal float32 Float_PIInverseFraction2 = float32(0.63661975)
  .field public static literal float32 Float_PIInverseFraction5 = float32(1.5915494)
  .field public static literal float32 Float_PITimes90 = float32(282.74335)
  .field public static literal float32 Float_PITimes180 = float32(565.48669)
  .field public static literal float32 Float_LooksLikePI = float32(3.1415925)
  .field public static literal float32 Float_LooksLikePI2 = float32(3.1415901)
  .field public static literal float32 Float_LooksLikePI3 = float32(3.141)
  .field public static literal float32 Float_BeforePI = float32(3.1415925)
  .field public static literal float32 Float_AfterPI = float32(3.141593)
  .field public static literal float32 Float_Negated_PI = float32(-3.1415927)
  .field public static literal float32 Float_Negated_HalfOfPI = float32(-1.5707964)
  .field public static literal float32 Float_Negated_QuarterOfPI = float32(-0.78539819)
  .field public static literal float32 Float_Negated_PITimes2 = float32(-6.2831855)
  .field public static literal float32 Float_Negated_3QuartersOfPI = float32(-2.3561945)
  .field public static literal float32 Float_Negated_PIDiv360 = float32(-8.7266462e-003)
  .field public static literal float32 Float_Negated_PIDiv16 = float32(-0.19634955)
  .field public static literal float32 Float_Negated_PIDiv32 = float32(-9.8174773e-002)
  .field public static literal float32 Float_Negated_PIInverseFraction = float32(-0.31830987)
  .field public static literal float32 Float_Negated_PIInverseFraction2 = float32(-0.63661975)
  .field public static literal float32 Float_Negated_PIInverseFraction5 = float32(-1.5915494)
  .field public static literal float32 Float_Negated_PITimes90 = float32(-282.74335)
  .field public static literal float32 Float_Negated_PITimes180 = float32(-565.48669)
  .field public static literal float32 Float_Negated_LooksLikePI = float32(-3.141)
  .field public static literal float32 Float_Negated_BeforePI = float32(-3.1415925)
  .field public static literal float32 Float_Negated_AfterPI = float32(-3.141593)
  .field public static literal float32 Float_E = float32(2.7182817)
  .field public static literal float32 Float_Negated_E = float32(-2.7182817)
  .field public static literal float64 Double_PI = float64(3.1415926535897931)
  .field public static literal float64 Double_HalfOfPI = float64(1.5707963267948966)
  .field public static literal float64 Double_QuarterOfPI = float64(0.78539816339744828)
  .field public static literal float64 Double_PITimes2 = float64(6.2831853071795862)
  .field public static literal float64 Double_3QuartersOfPI = float64(2.3561944901923448)
  .field public static literal float64 Double_PIDiv360 = float64(8.7266462599716477e-003)
  .field public static literal float64 Double_PIDiv16 = float64(0.19634954084936207)
  .field public static literal float64 Double_PIDiv32 = float64(9.8174770424681035e-002)
  .field public static literal float64 Double_PIInverseFraction = float64(0.31830988618379069)
  .field public static literal float64 Double_PIInverseFraction2 = float64(0.63661977236758138)
  .field public static literal float64 Double_PIInverseFraction5 = float64(1.5915494309189535)
  .field public static literal float64 Double_PITimes90 = float64(282.74333882308139)
  .field public static literal float64 Double_PITimes180 = float64(565.48667764616278)
  .field public static literal float64 Double_LooksLikePI = float64(3.1415926000000001)
  .field public static literal float64 Double_LooksLikePI2 = float64(3.1415899999999999)
  .field public static literal float64 Double_LooksLikePI3 = float64(3.141)
  .field public static literal float64 Double_BeforePI = float64(3.1415926535897927)
  .field public static literal float64 Double_AfterPI = float64(3.1415926535897936)
  .field public static literal float64 Double_Negated_PI = float64(-3.1415926535897931)
  .field public static literal float64 Double_Negated_HalfOfPI = float64(-1.5707963267948966)
  .field public static literal float64 Double_Negated_QuarterOfPI = float64(-0.78539816339744828)
  .field public static literal float64 Double_Negated_PITimes2 = float64(-6.2831853071795862)
  .field public static literal float64 Double_Negated_3QuartersOfPI = float64(-2.3561944901923448)
  .field public static literal float64 Double_Negated_PIDiv360 = float64(-8.7266462599716477e-003)
  .field public static literal float64 Double_Negated_PIDiv16 = float64(-0.19634954084936207)
  .field public static literal float64 Double_Negated_PIDiv32 = float64(-9.8174770424681035e-002)
  .field public static literal float64 Double_Negated_PIInverseFraction = float64(-0.31830988618379069)
  .field public static literal float64 Double_Negated_PIInverseFraction2 = float64(-0.63661977236758138)
  .field public static literal float64 Double_Negated_PIInverseFraction5 = float64(-1.5915494309189535)
  .field public static literal float64 Double_Negated_PITimes90 = float64(-282.74333882308139)
  .field public static literal float64 Double_Negated_PITimes180 = float64(-565.48667764616278)
  .field public static literal float64 Double_Negated_LooksLikePI = float64(-3.141)
  .field public static literal float64 Double_Negated_BeforePI = float64(-3.1415926535897927)
  .field public static literal float64 Double_Negated_AfterPI = float64(-3.1415926535897936)
  .field public static literal float64 Double_E = float64(2.7182818284590451)
  .field public static literal float64 Double_BeforeE = float64(2.7182818284590446)
  .field public static literal float64 Double_AfterE = float64(2.7182818284590455)
  .field public static literal float64 Double_Negated_E = float64(-2.7182818284590451)
  .field public static literal float64 Double_Negated_BeforeE = float64(-2.7182818284590446)
  .field public static literal float64 Double_Negated_AfterE = float64(-2.7182818284590455)
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
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
