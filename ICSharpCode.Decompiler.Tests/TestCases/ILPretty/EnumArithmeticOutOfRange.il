#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

// The constant operands below are out of range for the enums' underlying types, but their
// low bits do match a member: -501 truncates to UShortEnum.Val1 (0xfe0b) and 70000 to
// ByteEnum.Val1 (0x70). Reinterpreting them as those members would change the result of the
// subtraction, so the decompiler has to keep the integer arithmetic.

.class public auto ansi sealed UShortEnum
    extends [CORE_ASSEMBLY]System.Enum
{
    .field public specialname rtspecialname uint16 value__
    .field public static literal valuetype UShortEnum Val1 = uint16(0xfe0b)
}

.class public auto ansi sealed ByteEnum
    extends [CORE_ASSEMBLY]System.Enum
{
    .field public specialname rtspecialname uint8 value__
    .field public static literal valuetype ByteEnum Val1 = uint8(0x70)
}

.class public auto ansi abstract sealed beforefieldinit EnumArithmeticOutOfRange
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig static int32 SubtractFromUShortEnum (valuetype UShortEnum 'value') cil managed
    {
        .maxstack 8

        ldarg.0
        ldc.i4 -501
        sub
        ret
    }

    .method public hidebysig static int32 SubtractFromByteEnum (valuetype ByteEnum 'value') cil managed
    {
        .maxstack 8

        ldarg.0
        ldc.i4 70000
        sub
        ret
    }
}
