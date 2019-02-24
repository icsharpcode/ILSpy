



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly EnumTests.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module EnumTests.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested public SimpleEnum
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleEnum Item1 = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleEnum Item2 = int32(0x00000001)
  } // end of class SimpleEnum

  .class auto ansi sealed nested public LongBasedEnum
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int64 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/LongBasedEnum Item1 = int64(0x0)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/LongBasedEnum Item2 = int64(0x1)
  } // end of class LongBasedEnum

  .class auto ansi sealed nested public LongWithInitializers
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int64 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/LongWithInitializers Item1 = int64(0x0)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/LongWithInitializers Item2 = int64(0x14)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/LongWithInitializers Item3 = int64(0x15)
  } // end of class LongWithInitializers

  .class auto ansi sealed nested public ShortWithInitializers
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int16 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/ShortWithInitializers Item1 = int16(0x0000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/ShortWithInitializers Item2 = int16(0x0014)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/ShortWithInitializers Item3 = int16(0x0015)
  } // end of class ShortWithInitializers

  .class auto ansi sealed nested public ByteWithInitializers
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname uint8 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/ByteWithInitializers Item1 = uint8(0x00)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/ByteWithInitializers Item2 = uint8(0x14)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/ByteWithInitializers Item3 = uint8(0x15)
  } // end of class ByteWithInitializers

  .class auto ansi sealed nested public SimpleFlagsEnum
         extends [mscorlib]System.Enum
  {
    .custom instance void [mscorlib]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleFlagsEnum None = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleFlagsEnum Item1 = int32(0x00000001)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleFlagsEnum Item2 = int32(0x00000002)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleFlagsEnum Item3 = int32(0x00000004)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/SimpleFlagsEnum All = int32(0x00000007)
  } // end of class SimpleFlagsEnum

  .class auto ansi sealed nested public NegativeValueWithFlags
         extends [mscorlib]System.Enum
  {
    .custom instance void [mscorlib]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/NegativeValueWithFlags Value = int32(0x80000001)
  } // end of class NegativeValueWithFlags

  .class auto ansi sealed nested public NegativeValueWithoutFlags
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests/NegativeValueWithoutFlags Value = int32(0x80000001)
  } // end of class NegativeValueWithoutFlags

  .method public hidebysig instance valuetype [mscorlib]System.AttributeTargets 
          SingleEnumValue() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.4
    IL_0001:  ret
  } // end of method EnumTests::SingleEnumValue

  .method public hidebysig instance valuetype [mscorlib]System.AttributeTargets 
          TwoEnumValuesOr() cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.s   68
    IL_0002:  ret
  } // end of method EnumTests::TwoEnumValuesOr

  .method public hidebysig instance valuetype [mscorlib]System.AttributeTargets 
          ThreeEnumValuesOr() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldc.i4     0x844
    IL_0005:  ret
  } // end of method EnumTests::ThreeEnumValuesOr

  .method public hidebysig instance valuetype [mscorlib]System.AttributeTargets 
          UnknownEnumValue() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldc.i4     0xf4240
    IL_0005:  ret
  } // end of method EnumTests::UnknownEnumValue

  .method public hidebysig instance valuetype [mscorlib]System.AttributeTargets 
          EnumAllValue() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldc.i4     0x7fff
    IL_0005:  ret
  } // end of method EnumTests::EnumAllValue

  .method public hidebysig instance valuetype [mscorlib]System.AttributeTargets 
          EnumZeroValue() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method EnumTests::EnumZeroValue

  .method public hidebysig instance object 
          PreservingTypeWhenBoxed() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldc.i4     0x1000
    IL_0005:  box        [mscorlib]System.AttributeTargets
    IL_000a:  ret
  } // end of method EnumTests::PreservingTypeWhenBoxed

  .method public hidebysig instance object 
          PreservingTypeWhenBoxedTwoEnum() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldc.i4     0x1004
    IL_0005:  box        [mscorlib]System.AttributeTargets
    IL_000a:  ret
  } // end of method EnumTests::PreservingTypeWhenBoxedTwoEnum

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method EnumTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.EnumTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
