.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly Issue1389
{
  .hash algorithm 0x00008004
  .ver 1:0:4059:39717
}
.module Issue1389.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000003    //  ILONLY 32BITREQUIRED

.class public auto ansi beforefieldinit Issue1389.Program
	extends [mscorlib]System.Object
{
	// Methods
	.method /* 06000001 */ private hidebysig static 
		object GetObject () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 2 (0x2)
		.maxstack 8

		IL_0000: ldnull
		IL_0001: throw
	} // end of method Program::GetObject

	.method /* 06000002 */ private hidebysig static 
		void UnusedResultOfIsinst () cil managed 
	{
		// Method begins at RVA 0x2053
		// Code size 12 (0xc)
		.maxstack 8

		IL_0000: call object Issue1389.Program::GetObject() /* 06000001 */
		IL_0005: isinst [mscorlib]System.TypeCode /* 01000002 */
		IL_000a: pop
		IL_000b: ret
	} // end of method Program::UnusedResultOfIsinst

	.method /* 06000003 */ private hidebysig static 
		bool BoolResultOfIsinst () cil managed 
	{
		// Method begins at RVA 0x2060
		// Code size 14 (0xe)
		.maxstack 8

		IL_0000: call object Issue1389.Program::GetObject() /* 06000001 */
		IL_0005: isinst [mscorlib]System.TypeCode /* 01000002 */
		IL_000a: ldnull
		IL_000b: cgt.un
		IL_000d: ret
	} // end of method Program::BoolResultOfIsinst

	.method /* 06000004 */ private hidebysig static 
		object EnumResultOfIsinst (
			object A_0
		) cil managed 
	{
		// Method begins at RVA 0x206f
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: isinst [mscorlib]System.TypeCode /* 01000002 */
		IL_0006: ret
	} // end of method Program::EnumResultOfIsinst

} // end of class Issue1389.Program