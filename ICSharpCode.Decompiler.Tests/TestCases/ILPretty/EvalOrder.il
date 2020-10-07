#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.class private auto ansi beforefieldinit EvalOrder
    extends [System.Private.CoreLib]System.Object
{
    .field private valuetype SimpleStruct 'field'

    // Methods
    .method public hidebysig static 
	    void Test (
		    class EvalOrder p
	    ) cil managed 
    {
	    // Method begins at RVA 0x20f0
	    // Code size 14 (0xe)
	    .maxstack 8

	    ldarg.0
	    ldflda valuetype SimpleStruct EvalOrder::'field'
	    ldc.i4.1
	    call instance void SimpleStruct::.ctor(int32)
	    ret
    } // end of method EvalOrder::Test

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x206e
        // Code size 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Private.CoreLib]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method Example::.ctor

} // end of class Example

.class private sequential ansi sealed beforefieldinit SimpleStruct
	extends [System.Runtime]System.ValueType
{
	.pack 0
	.size 1

	// Methods
	.method public hidebysig specialname rtspecialname 
		instance void .ctor (
			int32 val
		) cil managed 
	{
		// Method begins at RVA 0x20f9
		// Code size 9 (0x9)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: call void [System.Console]System.Console::WriteLine(int32)
		IL_0007: nop
		IL_0008: ret
	} // end of method SimpleStruct::.ctor

} // end of class SimpleStruct
