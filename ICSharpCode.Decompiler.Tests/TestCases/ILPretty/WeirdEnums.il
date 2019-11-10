// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly BoolEnum
{
  .ver 1:0:0:0
}
.module BoolEnum.exe
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x000001C4B6C90000

.class public auto ansi sealed TestEnum.BooleanEnum
	   extends [mscorlib]System.Enum
{
	.field public specialname rtspecialname bool value__
	.field public static literal valuetype TestEnum.BooleanEnum Min = bool(false)
	.field public static literal valuetype TestEnum.BooleanEnum Zero = bool(false)
  .field public static literal valuetype TestEnum.BooleanEnum One = bool(true)
	.field public static literal valuetype TestEnum.BooleanEnum Max = uint8(255)
}

.class public auto ansi sealed TestEnum.NativeIntEnum
	   extends [mscorlib]System.Enum
{
	.field public specialname rtspecialname native int value__
	.field public static literal valuetype TestEnum.NativeIntEnum Zero = int64(0)
	.field public static literal valuetype TestEnum.NativeIntEnum One = int64(1)
	.field public static literal valuetype TestEnum.NativeIntEnum FortyTwo = int64(42)
}
