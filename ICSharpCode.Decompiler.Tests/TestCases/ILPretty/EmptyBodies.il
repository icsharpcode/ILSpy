#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.class private auto ansi beforefieldinit EmptyBodies
    extends [CORE_ASSEMBLY]System.Object
{
    // I cannot test a truly empty body because the assembler will automatically add a ret instruction.

    .method public hidebysig static void RetVoid () cil managed 
    {
	    .maxstack 8
		ret
    }

	.method public hidebysig static int32 RetInt () cil managed 
    {
	    .maxstack 8
		ret
    }

	.method public hidebysig static void Nop () cil managed 
    {
	    .maxstack 8
		nop
    }

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

} // end of class EmptyBodies
