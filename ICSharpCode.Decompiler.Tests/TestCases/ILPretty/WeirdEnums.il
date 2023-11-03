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

.class nested public auto ansi sealed TestEnum.EnumWithNestedClass
    extends [System.Runtime]System.Enum
{
        // Nested Types
    .class nested public auto ansi beforefieldinit NestedClass
        extends [mscorlib]System.Object
    {
        // Methods
        .method public hidebysig specialname rtspecialname 
            instance void .ctor () cil managed 
        {
            // Method begins at RVA 0x206c
            // Code size 8 (0x8)
            .maxstack 8

            IL_0000: ldarg.0
            IL_0001: call instance void [mscorlib]System.Object::.ctor()
            IL_0006: nop
            IL_0007: ret
        } // end of method TestEnum.NestedClass::.ctor

    } // end of class NestedClass

    // Fields
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype TestEnum.EnumWithNestedClass Zero = int32(0)
    .field public static literal valuetype TestEnum.EnumWithNestedClass One = int32(1)
    
} // end of class EnumWithNestedClass