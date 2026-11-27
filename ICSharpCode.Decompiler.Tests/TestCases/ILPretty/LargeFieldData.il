.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly LargeFieldData
{
  .ver 1:0:0:0
}
.module LargeFieldData.dll

.class public explicit ansi sealed LargeFieldData.Data256
	extends [mscorlib]System.ValueType
{
  .pack 1
  .size 256
}

.class public explicit ansi sealed LargeFieldData.Data257
	extends [mscorlib]System.ValueType
{
  .pack 1
  .size 257
}

.class public abstract auto ansi sealed LargeFieldData.Fields
	extends [mscorlib]System.Object
{
  // Multiple fields may map to the same data, so decompilation must not duplicate the entire blob in every comment.
  .field public static valuetype LargeFieldData.Data256 Complete at D
  .field public static valuetype LargeFieldData.Data257 Shortened at D
  .field public static valuetype LargeFieldData.Data257 ShortenedAlias at D
}

.data D = int8 (65) [257]
