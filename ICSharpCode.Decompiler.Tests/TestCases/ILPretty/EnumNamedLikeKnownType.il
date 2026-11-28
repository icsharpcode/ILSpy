.assembly extern mscorlib
{
	.publickeytoken = ( B7 7A 5C 56 19 34 E0 89 )
	.ver 4:0:0:0
}
.assembly EnumNamedLikeKnownType
{
	.ver 1:0:0:0
}
.module EnumNamedLikeKnownType.dll

// Each enum name matches its underlying primitive type's known System type name.
.class public auto ansi sealed System.String
	extends [mscorlib]System.Enum
{
	.field public specialname rtspecialname string value__
}

.class public auto ansi sealed System.Int32
	extends [mscorlib]System.Enum
{
	.field public specialname rtspecialname int32 value__
}
